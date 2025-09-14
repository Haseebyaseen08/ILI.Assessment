using Data.Repositories.ApiUsage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Shared.DTO.ApiUsage;
using Shared.Extensions;
using System.Threading.Channels;

namespace Application.BackgroundWorker
{
    public class LogApiUsage(ChannelReader<ApiUsageInformation> reader,
        ILogger<LogApiUsage> logger,
        IServiceScopeFactory serviceScopeFactory): BackgroundService
    {
        private readonly ChannelReader<ApiUsageInformation> _assignChannelReader = reader ?? throw new ArgumentNullException(nameof(reader));
        private readonly SemaphoreSlim _semaphore = new(50); // Allow max 50 parallel operations
        private DateTime _lastEventLogged = DateTime.MinValue;
        private int _logWithinMinute;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!_assignChannelReader.Completion.IsCompleted && await _assignChannelReader.WaitToReadAsync(stoppingToken).ConfigureAwait(false))
            {
                while (_assignChannelReader.TryRead(out var logApiUsage))
                {
                    // Create a local copy of the request
                    var request = logApiUsage;
                    
                    // Fire and forget with semaphore control
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await ProcessApiUsageLogAsync(request, stoppingToken).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError("LogApiUsage.ExecuteAsync(): Background service Error {Error} Request:{Request}", ex.Message, request.ToJson());
                        }
                    }, stoppingToken);

                }
            }
        }

        private async Task ProcessApiUsageLogAsync(ApiUsageInformation request, CancellationToken cancellationToken)
        {
            try
            {
                // Throttling based on maximum concurrency
                await _semaphore.WaitAsync(cancellationToken);

                // Additional throttling based on sending rate
                var now = DateTime.UtcNow;
                if (now - _lastEventLogged < TimeSpan.FromMinutes(1) && _logWithinMinute >= 1000)
                {
                    logger.LogWarning("LogApiUsage.ProcessApiUsageLogAsync().Throttling(): Log rate is reached to his Limit 1000 logs per minute");
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken); // Delay if rate limit is reached
                }
            
                logger.LogInformation("LogApiUsage.ProcessApiUsageLogAsync(): Background service processing Request:{Request}", request.ToJson());
                
                // Create a scope to access scoped services
                using var scope = serviceScopeFactory.CreateScope();
                var apiUsageRepository = scope.ServiceProvider.GetRequiredService<IApiUsageRepository>();
                
                var response = await apiUsageRepository.AddApiUsageLogAsync(request).ConfigureAwait(false);
                logger.LogInformation("LogApiUsage.ProcessApiUsageLogAsync(): Background service processed Request:{Request} Response:{Response}", request.ToJson(), response);

                _lastEventLogged = now;
                _logWithinMinute++;
                if (now.Minute != _lastEventLogged.Minute)
                {
                    _logWithinMinute = 0;
                }

            }
            catch (Exception e)
            {
                logger.LogError("LogApiUsage.ProcessApiUsageLogAsync(): Background service Error {Error} Request:{Request}", e.Message, request.ToJson());
            }
            finally
            {
                _semaphore.Release();
            }
        }

    }
}
