using System.Security.Claims;
using DevSAAS.Core.Identity.Services;
using DevSAAS.Core.Localization.Services;
using DevSAAS.Web.Responses;
using DevSAAS.Web.Auth.Models;
using DevSAAS.Web.Auth.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevSAAS.Web.Auth.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public AuthController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login credentials)
        {
            var user = await _authService.LoginAsync(credentials.Username, credentials.Password);
            if (user is null)
            {
                return ApiResponse.Send(404, "error", LanguageService.Get("LoginError"));
            }
            
            var token = _authService.CreateToken(user);
            return ApiResponse.Send(200, "success", LanguageService.Get("Welcome")  + " " + user.Name, new AuthResponse(user.Id,  user.Photo, user.Name, user.Email, user.EmailVerifiedAt, user.Phone, user.PhoneVerifiedAt, token));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register credentials)
        {
            try
            {
                var user = await _authService.RegisterAsync(credentials.Name, credentials.Email, credentials.Phone, credentials.Password);
                if (user is null)
                {
                    return ApiResponse.Send(500, "error", LanguageService.Get("CreateError").Replace(":object", LanguageService.Get("Account")));
                }
                
                var token = _authService.CreateToken(user);
                return ApiResponse.Send(201, "success", LanguageService.Get("Welcome")  + " " + user.Name, new AuthResponse(user.Id,  user.Photo, user.Name, user.Email, user.EmailVerifiedAt, user.Phone, user.PhoneVerifiedAt, token));
            }
            catch (Exception e)
            {
                return ApiResponse.Send(400, "error", e.Message);
            }
        }
        
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return ApiResponse.Send(401, "error", LanguageService.Get("Unauthorized"));
            }

            var claims = identity.Claims;
            var userId = claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (userId == null)
            {
                return ApiResponse.Send(401, "error", LanguageService.Get("Unauthorized"));
            }

            var user = await _userService.GetByIdAsync(userId);
            return user == null 
                ? ApiResponse.Send(401, "error", LanguageService.Get("Unauthorized")) 
                : ApiResponse.Send(200, "success", "Profile Loaded", new ProfileResponse(user));
        }
        
        
    }
}
