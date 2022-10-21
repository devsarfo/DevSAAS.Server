using DevSAAS.Core.Configuration.Stores;
using DevSAAS.Core.Database;
using DevSAAS.Core.Localization.Services;
using DevSAAS.Core.Notification.Providers;

namespace DevSAAS.Core.Notification.Services;

public class MailService
{
    private const string _key = "email_gateway";
    private readonly DatabaseFactory _databaseFactory;
    
    public MailService(DatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }
    
    public async Task<bool> SendAsync(string destination, string subject, string message)
    {
        var destinations = new string[] { destination };
        return await SendAsync(destinations, subject, message);
    }
    
    public async Task<bool> SendAsync(string[] destinations, string subject, string message)
    {
        await using var conn = _databaseFactory.Instance();
        
        try
        {
            var settingStore = new SettingStore(conn);
            var email = await settingStore.GetByKeyAsync(_key);

            if (email is null)
            {
                throw new Exception(LanguageService.Get("ConfigurationNotFound") + ": E-mail");
            }

            var emailGateway = email.Value;
            if (emailGateway.Equals("gmail", StringComparison.InvariantCultureIgnoreCase))
            {
                return await new Gmail(settingStore).Send(destinations, subject, message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return false;
    }
}