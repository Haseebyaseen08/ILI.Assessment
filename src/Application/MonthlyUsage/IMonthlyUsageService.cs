using Shared.DTO.ApiUsage;

namespace Application.MonthlyUsage
{
    public interface IMonthlyUsageService
    {
        Task<List<MonthlyUsageSummaryDto>> GetCustomerMonthlySummariesAsync(int customerId);
        Task<MonthlyUsageSummaryDto?> GetCustomerMonthlySummaryAsync(int customerId, int year, int month);
        Task<List<MonthlyUsageSummaryDto>> GetCustomerMonthlySummariesForPeriodAsync(int customerId, DateTime startDate, DateTime endDate);
        Task<bool> RegenerateMonthlySummaryAsync(int customerId, int year, int month);
        Task<bool> GenerateMonthlySummaryForAllCustomersAsync(int year, int month);
    }
}