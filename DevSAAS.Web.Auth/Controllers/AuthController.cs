using DevSAAS.Core.Identity.Entities;
using DevSAAS.Core.Identity.Services;
using DevSAAS.Core.Responses;
using DevSAAS.Web.Auth.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevSAAS.Web.Auth.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login credentials)
        {
            var user = await _userService.Login(credentials.Username, credentials.Password);
            var result = new ApiResponse<User>("success", "Welcome DevSarfo", user);
            return StatusCode(200, result);
        }
        
        
        [HttpPost("register")]
        public IActionResult Register([FromBody] Register credentials)
        {
            // Register
            var result = new ApiResponse<string>("success", "Welcome DevSarfo");
            return StatusCode(200, result);
        }
        
    }
}
