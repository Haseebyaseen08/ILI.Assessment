using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.JWT;
using Shared.Settings;

namespace Shared
{
    public static class Startup
    {
        public static IServiceCollection ConfigureSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure JWT settings
            services.Configure<JWTSettings>(configuration.GetSection(nameof(JWTSettings)));

            services.Configure<RateLimitOptions>(configuration.GetSection(nameof(RateLimitOptions)));


            // Register JWT service

            services.AddTransient<IJWTService, JWTService>();

            return services;
        }
    }
}
