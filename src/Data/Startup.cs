using Data.Context;
using Data.Repositories.ApiUsage;
using Data.Repositories.Customer;
using Data.Repositories.MonthlyUsage;
using Data.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data
{
    public static class Startup
    {
        public static IServiceCollection ConfigureDataServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Register DbContext with SQL Server
            services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.MigrationsAssembly("Data")));

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IApiUsageRepository, ApiUsageRepository>();
            services.AddTransient<IMonthlyUsageSummaryRepository, MonthlyUsageSummaryRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();

            return services;
        }
    }
}
