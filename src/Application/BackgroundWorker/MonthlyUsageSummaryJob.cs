using Application.MonthlyUsage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.BackgroundWorker
{
    public class MonthlyUsageSummaryJob : BackgroundService
    {
        private readonly ILogger<MonthlyUsageSummaryJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Check daily

        public MonthlyUsageSummaryJob(
            ILogger<MonthlyUsageSummaryJob> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Monthly Usage Summary background job started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessMonthlyUsageSummaryAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing monthly usage summary");
                }

                // Wait for the next iteration
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Monthly Usage Summary background job stopped");
        }

        private async Task ProcessMonthlyUsageSummaryAsync()
        {
            var now = DateTime.UtcNow;
            
            // Check if it's the first day of the month and we haven't processed this month yet
            if (now.Day == 1)
            {
                var previousMonth = now.AddMonths(-1);
                var year = previousMonth.Year;
                var month = previousMonth.Month;

                _logger.LogInformation("Processing monthly usage summary for {Year}-{Month:D2}", year, month);

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var monthlyUsageService = scope.ServiceProvider.GetRequiredService<IMonthlyUsageService>();

                    var success = await monthlyUsageService.GenerateMonthlySummaryForAllCustomersAsync(year, month);

                    if (success)
                    {
                        _logger.LogInformation("Successfully generated monthly usage summaries for {Year}-{Month:D2}", year, month);
                    }
                    else
                    {
                        _logger.LogWarning("Some monthly usage summaries failed to generate for {Year}-{Month:D2}", year, month);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating monthly usage summaries for {Year}-{Month:D2}", year, month);
                }
            }
            else
            {
                _logger.LogDebug("Not the first day of month, skipping monthly usage summary generation");
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Monthly Usage Summary background job is starting");
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Monthly Usage Summary background job is stopping");
            await base.StopAsync(cancellationToken);
        }
    }
}