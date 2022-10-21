using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Entities;
using DevSAAS.Core.Identity.Stores;
using DevSAAS.Core.Notification.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DevSAAS.Core.Identity.Services;

public class AuthService
{
    private readonly DatabaseFactory _databaseFactory;
    private readonly IConfiguration _configuration;
    private SmsService _smsService;
    private MailService _mailService;

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
        
        
        return changes > 0 ? user : null;
    }

    public string? CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new ("UserId", user.Id),
            new ("Photo", user.Photo),
            new ("Name", user.Name),
            new ("Email", user.Email),
            new ("Phone", user.Phone)
        };
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        
        return jwt;
    }
}