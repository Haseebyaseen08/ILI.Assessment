using Data.Entities;

namespace Data.Repositories.Customer
{
    public interface ICustomerRepository
    {
        Task<List<Entities.Customer>> GetActiveCustomersAsync();
        Task<Entities.Customer?> GetByIdAsync(int customerId);
        Task<List<Entities.Customer>> GetCustomersByIdsAsync(IEnumerable<int> customerIds);
    }
}