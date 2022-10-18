using DevSAAS.Core.Identity.Entities;
using DevSAAS.Core.Identity.Services;
using DevSAAS.Core.Localization.Services;
using DevSAAS.Web.Responses;
using DevSAAS.Web.Auth.Models;
using DevSAAS.Web.Auth.Responses;
using DevSAAS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DevSAAS.Web.Auth.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LanguageService _languageService;
        private readonly AuthService _authService;

        public AuthController(AuthService authService, LanguageService languageService)
        {
            _authService = authService;
            _languageService = languageService;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login credentials)
        {
            var user = await _authService.Login(credentials.Username, credentials.Password);
            if (user is null)
            {
                return ApiResponse.Send(404, "error", _languageService.Get("LoginError"));
            }
            
            //Generate Token
            var token = _authService.CreateToken(user);
            return ApiResponse.Send(200, "success", _languageService.Get("Welcome")  + " " + user.Name, new AuthResponse(user.Id,  user.Photo, user.Name, user.Email, user.EmailVerifiedAt, user.Phone, user.PhoneVerifiedAt, token));
        }
        
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register credentials)
        {
            // Register
            var results = await _authService.Register(credentials.Name, credentials.Email, credentials.Phone, credentials.Password);

            return results
                ? ApiResponse.Send(201, "success", _languageService.Get("Welcome") + " " + credentials.Name)
                : ApiResponse.Send(404, "error", _languageService.Get("CreateError"));
        }
        
    }
}
