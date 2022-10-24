using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Entities;
using DevSAAS.Core.Identity.Stores;
using DevSAAS.Core.Notification.Services;
using DevSAAS.Core.Notification.Templates.Verification;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DevSAAS.Core.Identity.Services;

public class AuthService
{
    private readonly DatabaseFactory _databaseFactory;
    private readonly IConfiguration _configuration;
    private readonly SmsService _smsService;
    private readonly MailService _mailService;

    public AuthService(DatabaseFactory databaseFactory, IConfiguration configuration, SmsService smsService, MailService mailService)
    {
        _databaseFactory = databaseFactory;
        _configuration = configuration;
        _smsService = smsService;
        _mailService = mailService;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        await using var conn = _databaseFactory.Instance();
        var userStore = new UserStore(conn);
        var user = await userStore.GetByUsernameAsync(username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return null;
        }

        return user;
    }
    
    public async Task<User?> RegisterAsync(string name, string email, string phone, string password)
    {
        await using var conn = _databaseFactory.Instance();
        var userStore = new UserStore(conn);
        
        //Check if E-mail exists
        if (await userStore.GetByEmailAsync(email) is not null)
        {
            throw new ApplicationException("E-mail Address Already Exists!");
        }
        
        //Check if or Phone exists
        if (await userStore.GetByPhoneAsync(phone) is not null)
        {
            throw new ApplicationException("Phone Number Already Exists!");
        }
        
        //Create User
        var user = new User(name, email, phone, password);
        var changes = await userStore.InsertAsync(user);

        // Send OTP
        var otpStore = new OtpStore(conn);
        var otp = await otpStore.GenerateCode(user.Id);

        new VerificationCode(_smsService, _mailService, otp, user.Phone, user.Email).Send();
        
        return changes > 0 ? user : null;
    }

    public string? CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new ("Id", user.Id),
            new ("Photo", user.Photo),
            new ("Name", user.Name),
            new ("Email", user.Email),
            new ("Phone", user.Phone)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
        );
        
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        
        return jwt;
    }
}