using Application.BackgroundWorker;
using Application.MonthlyUsage;
using Application.User;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.ApiUsage;
using System.Threading.Channels;

namespace Application
{
    public static class Startup
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMonthlyUsageService, MonthlyUsageService>();

            // Registering unbounded channels for LogApiUsage
            var channel = Channel.CreateUnbounded<ApiUsageInformation>();
            services.AddSingleton(_ => channel.Writer);
            services.AddSingleton(_ => channel.Reader);

            services.AddHostedService<LogApiUsage>();
            services.AddHostedService<MonthlyUsageSummaryJob>();

            return services;
        }
    }
}
