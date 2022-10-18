using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Services;
using DevSAAS.Core.Localization.Services;

namespace DevSAAS.Web.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddDevSaasServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<DatabaseFactory>();
        builder.Services.AddSingleton<LanguageService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<UserService>();
        
        return builder;
    }
}