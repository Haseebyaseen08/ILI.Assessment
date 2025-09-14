using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.ApiUsage;

namespace Data.Repositories.ApiUsage
{
    public class ApiUsageRepository(DBContext dbContext) : IApiUsageRepository
    {
        public async Task<bool> AddApiUsageLogAsync(ApiUsageInformation apiUsageInformation)
        {
            await dbContext.ApiUsageLogs.AddAsync(new ApiUsageLog
            {
                CustomerId = apiUsageInformation.CustomerId,
                UserId = apiUsageInformation.UserId,
                Endpoint = apiUsageInformation.Endpoint,
                Timestamp = apiUsageInformation.Timestamp
            });
            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<int> GetMonthlyUsageCountAsync(int customerId, int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);

            return await dbContext.ApiUsageLogs
                .Where(log => log.CustomerId == customerId && 
                             log.Timestamp >= monthStart && 
                             log.Timestamp < monthEnd)
                .CountAsync();
        }

        public async Task<Dictionary<string, int>> GetMonthlyEndpointUsageAsync(int customerId, int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);

            return await dbContext.ApiUsageLogs
                .Where(log => log.CustomerId == customerId && 
                             log.Timestamp >= monthStart && 
                             log.Timestamp < monthEnd)
                .GroupBy(log => log.Endpoint)
                .Select(g => new { Endpoint = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Endpoint, x => x.Count);
        }

        public async Task<Dictionary<int, int>> GetMonthlyUsageCountsForCustomersAsync(IEnumerable<int> customerIds, int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);

            return await dbContext.ApiUsageLogs
                .Where(log => customerIds.Contains(log.CustomerId) && 
                             log.Timestamp >= monthStart && 
                             log.Timestamp < monthEnd)
                .GroupBy(log => log.CustomerId)
                .Select(g => new { CustomerId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CustomerId, x => x.Count);
        }
    }
}