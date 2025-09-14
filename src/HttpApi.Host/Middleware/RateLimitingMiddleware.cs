using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shared.Constant;
using Shared.DTO.ApiUsage;
using Shared.JWT;
using Shared.Settings;
using System.Security.Claims;
using System.Threading.Channels;

namespace HttpApi.Host.Middleware
{
    public class RateLimitingMiddleware(
        RequestDelegate next,
        IMemoryCache cache,
        IOptions<RateLimitOptions> rateLimitOptions,
        ILogger<RateLimitingMiddleware> logger,
        IJWTService jwtService,
        ChannelWriter<ApiUsageInformation> channelWriter
        )
    {
        private readonly RateLimitOptions _rateLimitOptions = rateLimitOptions.Value;
        
        public async Task InvokeAsync(HttpContext context)
        {
            // Skip rate limiting for User controller (login, etc.)
            if (context.Request.Path.StartsWithSegments("/api/User", StringComparison.OrdinalIgnoreCase))
            {
                await next(context);
                return;
            }

            // Extract JWT and validate
            var token = ExtractTokenFromHeader(context);
            if (string.IsNullOrEmpty(token))
            {
                await next(context);
                return;
            }

            var claimsPrincipal = jwtService.ValidateToken(token);
            if (claimsPrincipal == null)
            {
                await next(context);
                return;
            }

            // Extract user information from JWT
            var userId = GetClaimValue(claimsPrincipal, "UserId");
            var customerId = GetClaimValue(claimsPrincipal, "CustomerId");
            var subscriptionPlan = GetClaimValue(claimsPrincipal, "CustomerSubscriptionPlan");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(subscriptionPlan))
            {
                await next(context);
                return;
            }

            var userIdInt = int.Parse(userId);

            // Check rate limit (per-second)
            if (!CheckRateLimitAsync(userId, subscriptionPlan))
            {
                logger.LogWarning("Rate limit exceeded for User: {UserId}, Plan: {SubscriptionPlan}", userId, subscriptionPlan);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync($"{{\"message\":\"{AppMessages.RateLimitExceeded}\"}}");
                return;
            }

            // Log API usage to channel
            await LogApiUsageAsync(userId, customerId, context.Request.Path);

            await next(context);
        }

        private string? ExtractTokenFromHeader(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return authorizationHeader["Bearer ".Length..].Trim();
        }

        private static string? GetClaimValue(ClaimsPrincipal claimsPrincipal, string claimType)
        {
            return claimsPrincipal.FindFirst(claimType)?.Value;
        }

        private bool CheckRateLimitAsync(string userId, string subscriptionPlan)
        {
            if (!_rateLimitOptions.Plans.TryGetValue(subscriptionPlan, out var rateLimitConfig))
            {
                logger.LogWarning("Rate limit plan not found: {SubscriptionPlan}", subscriptionPlan);
                return true; // Allow if plan not found
            }

            var cacheKey = $"rate_limit_{userId}";
            var now = DateTimeOffset.UtcNow;
            var windowStart = now.AddSeconds(-1); // 1-second sliding window

            // Get or create the request timestamps list
            var requestTimestamps = cache.Get<List<DateTimeOffset>>(cacheKey) ?? new List<DateTimeOffset>();

            // Remove old requests outside the sliding window
            requestTimestamps.RemoveAll(timestamp => timestamp < windowStart);

            // Check if adding this request would exceed the rate limit
            if (requestTimestamps.Count >= rateLimitConfig.RequestsPerSecond)
            {
                return false; // Rate limit exceeded
            }

            // Add current request timestamp
            requestTimestamps.Add(now);

            // Update cache with sliding expiration
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2), // Keep for 2 seconds to handle sliding window
                SlidingExpiration = TimeSpan.FromSeconds(1)
            };

            cache.Set(cacheKey, requestTimestamps, cacheEntryOptions);

            logger.LogDebug("Rate limit check passed for User: {UserId}, Current requests: {CurrentRequests}/{MaxRequests}", 
                userId, requestTimestamps.Count, rateLimitConfig.RequestsPerSecond);

            return true;
        }

        private async Task LogApiUsageAsync(string userId, string customerId, string endpoint)
        {
            try
            {
                await channelWriter.WriteAsync(new()
                {
                    UserId = int.Parse(userId),
                    CustomerId = int.Parse(customerId),
                    Endpoint = endpoint,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to write API usage information to channel");
            }
        }
    }
}
