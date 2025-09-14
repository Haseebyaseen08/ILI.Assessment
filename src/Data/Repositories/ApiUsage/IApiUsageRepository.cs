using Shared.DTO.ApiUsage;
using Data.Entities;

namespace Data.Repositories.ApiUsage
{
    public interface IApiUsageRepository
    {
        Task<bool> AddApiUsageLogAsync(ApiUsageInformation apiUsageInformation);
        Task<int> GetMonthlyUsageCountAsync(int customerId, int year, int month);
        Task<Dictionary<string, int>> GetMonthlyEndpointUsageAsync(int customerId, int year, int month);
        Task<Dictionary<int, int>> GetMonthlyUsageCountsForCustomersAsync(IEnumerable<int> customerIds, int year, int month);
    }
}