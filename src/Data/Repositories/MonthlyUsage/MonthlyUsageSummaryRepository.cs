using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.MonthlyUsage
{
    public class MonthlyUsageSummaryRepository : IMonthlyUsageSummaryRepository
    {
        private readonly DBContext _dbContext;

        public MonthlyUsageSummaryRepository(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MonthlyUsageSummary?> GetByCustomerAndPeriodAsync(int customerId, int year, int month)
        {
            return await _dbContext.MonthlyUsageSummaries
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.CustomerId == customerId && s.Year == year && s.Month == month);
        }

        public async Task<List<MonthlyUsageSummary>> GetByCustomerAsync(int customerId)
        {
            return await _dbContext.MonthlyUsageSummaries
                .Include(s => s.Customer)
                .Where(s => s.CustomerId == customerId)
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Month)
                .ToListAsync();
        }

        public async Task<List<MonthlyUsageSummary>> GetByCustomerForPeriodAsync(int customerId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.MonthlyUsageSummaries
                .Include(s => s.Customer)
                .Where(s => s.CustomerId == customerId &&
                           (s.Year > startDate.Year || (s.Year == startDate.Year && s.Month >= startDate.Month)) &&
                           (s.Year < endDate.Year || (s.Year == endDate.Year && s.Month <= endDate.Month)))
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Month)
                .ToListAsync();
        }

        public async Task<bool> CreateOrUpdateSummaryAsync(MonthlyUsageSummary summary)
        {
            var existing = await GetByCustomerAndPeriodAsync(summary.CustomerId, summary.Year, summary.Month);
            
            if (existing != null)
            {
                existing.TotalApiCalls = summary.TotalApiCalls;
                existing.PricePerCall = summary.PricePerCall;
                existing.TotalCost = summary.TotalCost;
                existing.EndpointUsageJson = summary.EndpointUsageJson;
                existing.UpdatedDate = DateTime.UtcNow;
                
                _dbContext.MonthlyUsageSummaries.Update(existing);
            }
            else
            {
                await _dbContext.MonthlyUsageSummaries.AddAsync(summary);
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(int customerId, int year, int month)
        {
            return await _dbContext.MonthlyUsageSummaries
                .AnyAsync(s => s.CustomerId == customerId && s.Year == year && s.Month == month);
        }

        public async Task<List<MonthlyUsageSummary>> GetAllSummariesForPeriodAsync(int year, int month)
        {
            return await _dbContext.MonthlyUsageSummaries
                .Include(s => s.Customer)
                .Where(s => s.Year == year && s.Month == month)
                .ToListAsync();
        }
    }
}