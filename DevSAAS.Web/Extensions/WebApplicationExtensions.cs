using DevSAAS.Core.Database;

namespace DevSAAS.Web.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddDevSaasServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<DatabaseFactory>();

        return builder;
    }
}