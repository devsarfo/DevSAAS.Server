using System.Text;
using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Services;
using DevSAAS.Core.Notification.Services;
using DevSAAS.Web.Responses;
using DevSAAS.Web.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DevSAAS.Web.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddDevSaasServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = RequestValidator.MakeValidationResponse;
        }); 
        
        builder.Services.AddSingleton<DatabaseFactory>();
        builder.Services.AddSingleton<SmsService>();
        builder.Services.AddSingleton<MailService>();
        
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<UserService>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

        return builder;
    }
}