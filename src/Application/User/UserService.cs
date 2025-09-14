using Data.Repositories.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Constant;
using Shared.DTO;
using Shared.DTO.User;
using Shared.JWT;

namespace Application.User
{
    public class UserService (IUserRepository userRepository, IJWTService jwtService, ILogger<UserService> logger): IUserService
    {
        public async Task<Response<LoginResponse>> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var user = await userRepository.GetByEmailAsync(loginRequest.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                {
                    return new()
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = AppMessages.InvalidCredentials
                    };
                }

                var token = jwtService.GenerateToken(
                    user!.Id,
                    user.CustomerId,
                    user.SubscriptionType.ToString(),
                    user.Email,
                    user.FirstName,
                    user.LastName
                );

                return new()
                {
                    Data = new()
                    {
                        Token = token,
                        User = user
                    },
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during login for email: {Email}", loginRequest.Email);
                return new()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = AppMessages.InternalServerError
                };
            }
        }

        
    }
}