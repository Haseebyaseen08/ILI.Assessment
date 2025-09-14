using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HttpApi.Host.Controllers
{
    /// <summary>
    /// Base controller for API controllers in the application.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController:Controller
    {
    }
}
