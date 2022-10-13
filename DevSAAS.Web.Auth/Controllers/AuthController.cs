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
            var result = user == null 
                ? new ApiResponse<User>("error", "Invalid Username or Password")
                : new ApiResponse<User>("success", "Welcome " + user.Name, user);
            
            return StatusCode(200, result);
        }
        
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register credentials)
        {
            // Register
            var user = await _userService.Register(credentials.Name, credentials.Email, credentials.Phone, credentials.Password);
            
            var result = user == null 
                ? new ApiResponse<User>("error", "An error occured whiles creating account")
                : new ApiResponse<User>("success", "Welcome " + credentials.Name);
            
            return StatusCode(201, result);
        }
        
    }
}
