using Shared.DTO;
using Shared.DTO.User;

namespace Application.User
{
    public interface IUserService
    {
        Task<Response<LoginResponse>> LoginAsync(LoginRequest loginRequest);
    }
}
