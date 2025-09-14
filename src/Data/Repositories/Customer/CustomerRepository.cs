using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Customer
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DBContext _dbContext;

        public CustomerRepository(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Entities.Customer>> GetActiveCustomersAsync()
        {
            return await _dbContext.Customers
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Entities.Customer?> GetByIdAsync(int customerId)
        {
            return await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<List<Entities.Customer>> GetCustomersByIdsAsync(IEnumerable<int> customerIds)
        {
            return await _dbContext.Customers
                .Where(c => customerIds.Contains(c.Id))
                .ToListAsync();
        }
    }
}