using Data.Entities;
using Shared.DTO.User;

namespace Data.Repositories.Users
{
    public interface IUserRepository
    {
        Task<UserInformation?> GetByEmailAsync(string email);
    }
}
