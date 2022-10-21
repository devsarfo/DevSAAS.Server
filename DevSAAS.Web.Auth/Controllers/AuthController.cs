using DevSAAS.Core.Identity.Entities;
using DevSAAS.Core.Identity.Services;
using DevSAAS.Core.Localization.Services;
using DevSAAS.Web.Responses;
using DevSAAS.Web.Auth.Models;
using DevSAAS.Web.Auth.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DevSAAS.Web.Auth.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
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
                    return ApiResponse.Send(500, "success", LanguageService.Get("CreateError").Replace(":object", LanguageService.Get("Account")));
                }
                
                var token = _authService.CreateToken(user);
                return ApiResponse.Send(201, "success", LanguageService.Get("Welcome")  + " " + user.Name, new AuthResponse(user.Id,  user.Photo, user.Name, user.Email, user.EmailVerifiedAt, user.Phone, user.PhoneVerifiedAt, token));
            }
            catch (Exception e)
            {
                return ApiResponse.Send(400, "error", e.Message);
            }
        }
        
    }
}
