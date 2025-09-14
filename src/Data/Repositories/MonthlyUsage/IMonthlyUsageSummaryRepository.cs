using Data.Entities;
using Shared.DTO.ApiUsage;

namespace Data.Repositories.MonthlyUsage
{
    public interface IMonthlyUsageSummaryRepository
    {
        Task<MonthlyUsageSummary?> GetByCustomerAndPeriodAsync(int customerId, int year, int month);
        Task<List<MonthlyUsageSummary>> GetByCustomerAsync(int customerId);
        Task<List<MonthlyUsageSummary>> GetByCustomerForPeriodAsync(int customerId, DateTime startDate, DateTime endDate);
        Task<bool> CreateOrUpdateSummaryAsync(MonthlyUsageSummary summary);
        Task<bool> ExistsAsync(int customerId, int year, int month);
        Task<List<MonthlyUsageSummary>> GetAllSummariesForPeriodAsync(int year, int month);
    }
}