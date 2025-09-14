using Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.User;
using Shared.ResponseHandler;

namespace HttpApi.Host.Controllers
{
    public class UserController(IUserService userService):BaseController
    {
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
            => ResponseHandler<LoginResponse>.Response(await userService.LoginAsync(loginRequest));
    }
}
