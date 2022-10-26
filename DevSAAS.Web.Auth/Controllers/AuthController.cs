using DevSAAS.Core.Identity.Services;
using DevSAAS.Core.Localization.Services;
using DevSAAS.Web.Responses;
using DevSAAS.Web.Auth.Models;
using DevSAAS.Web.Auth.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevSAAS.Web.Auth.Controllers;

[Route("/")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserService _userService;
    private readonly RoleService _roleService;
    private readonly OtpService _otpService;

    public AuthController(AuthService authService, UserService userService, OtpService otpService, RoleService roleService)
    {
        _authService = authService;
        _userService = userService;
        _otpService = otpService;
        _roleService = roleService;
    }

    [HttpGet]
    public ContentResult Index()
    {
        return base.Content("<h1>API is up and running. Read <a href='swagger/index.html'>Docs</a></1>", "text/html");
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login credentials)
    {
        var user = await _authService.LoginAsync(credentials.Username, credentials.Password);
        if (user is null)
        {
            return ApiResponse.Send(400, "error", LanguageService.Get("LoginError"));
        }

        var token = _authService.CreateToken(user);
        return ApiResponse.Send(200, "success", LanguageService.Get("Welcome") + " " + user.Name,
            new AuthResponse(user.Id, user.Photo, user.Name, user.Email, user.EmailVerifiedAt, user.Phone,
                user.PhoneVerifiedAt, token));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Register credentials)
    {
        try
        {
            var user = await _authService.RegisterAsync(credentials.Name, credentials.Email, credentials.Phone,
                credentials.Password);
            if (user is null)
            {
                return ApiResponse.Send(500, "error",
                    LanguageService.Get("CreateError").Replace(":object", LanguageService.Get("Account")));
            }

            var token = _authService.CreateToken(user);
            return ApiResponse.Send(201, "success", LanguageService.Get("Welcome") + " " + user.Name,
                new AuthResponse(user.Id, user.Photo, user.Name, user.Email, user.EmailVerifiedAt, user.Phone,
                    user.PhoneVerifiedAt, token));
        }
        catch (Exception e)
        {
            return ApiResponse.Send(400, "error", e.Message);
        }
    }

    [HttpGet("profile"), Authorize]
    public async Task<IActionResult> Profile()
    {
        try
        {
            var user = await _authService.GetUserAsync(HttpContext);
            return user is null ? ApiResponse.Send(500, "error", LanguageService.Get("RecordNotFound").Replace(":object", LanguageService.Get("Account"))) : ApiResponse.Send(200, "success", "Profile Loaded", new ProfileResponse(user));
        }
        catch (Exception e)
        {
            return ApiResponse.Send(400, "error", e.Message);
        }
    }

    [HttpPost("otp/verify"), Authorize]
    public async Task<IActionResult> VerifyOtp([FromBody] Otp otp)
    {
        try
        {
            var user = await _authService.GetUserAsync(HttpContext);
            if (user is null)
            {
                return ApiResponse.Send(500, "error",
                    LanguageService.Get("RecordNotFound").Replace(":object", LanguageService.Get("Account")));
            }

            var verify = await _otpService.Verify(user.Id, otp.Code);

            return !verify
                ? ApiResponse.Send(400, "error", LanguageService.Get("InvalidOtp"))
                : ApiResponse.Send(200, "success", LanguageService.Get("AccountVerified"));
        }
        catch (Exception e)
        {
            return ApiResponse.Send(400, "error", e.Message);
        }
    }

    [HttpPost("otp/resend"), Authorize]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtp resendOtp)
    {
        try
        {
            var user = await _authService.GetUserAsync(HttpContext);
            if (user is null)
            {
                return ApiResponse.Send(500, "error",
                    LanguageService.Get("RecordNotFound").Replace(":object", LanguageService.Get("Account")));
            }

            var resend = await _otpService.Resend(user.Id, resendOtp.Phone);

            
            return resend
                ? ApiResponse.Send(200, "success", LanguageService.Get("VerificationCodeSentPhone"))
                : ApiResponse.Send(400, "error",
                    LanguageService.Get("SendError").Replace(":object", LanguageService.Get("VerificationCode")));
        }
        catch (Exception e)
        {
            return ApiResponse.Send(400, "error", e.Message);
        }
    }
}