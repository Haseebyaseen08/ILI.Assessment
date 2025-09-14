using Data.Context;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.User;

namespace Data.Repositories.Users
{
    public class UserRepository (DBContext dBContext): IUserRepository
    {
        public async Task<UserInformation?> GetByEmailAsync(string email)
        {
            var response =  await dBContext.Users
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Email == email);

            if(response is null)
                return null;

            return new()
            {
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Id = response.Id,
                SubscriptionType = response.Customer!.SubscriptionType,
                CustomerId = response.CustomerId,
                Password = response.Password
            };
        }
    }
}
