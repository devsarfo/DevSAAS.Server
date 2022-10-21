using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Services;
using DevSAAS.Core.Notification.Services;
using DevSAAS.Web.Exceptions;
using DevSAAS.Web.Validation;
using FluentValidation.AspNetCore;

namespace DevSAAS.Web.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddDevSaasServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddControllers().AddFluentValidation().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = RequestValidator.MakeValidationResponse;
        }); 
        
        builder.Services.AddSingleton<DatabaseFactory>();
        builder.Services.AddSingleton<SmsService>();
        builder.Services.AddSingleton<MailService>();
        
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<UserService>();

        return builder;
    }
}