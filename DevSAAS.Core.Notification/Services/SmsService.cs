using DevSAAS.Core.Configuration.Stores;
using DevSAAS.Core.Database;
using DevSAAS.Core.Localization.Services;
using DevSAAS.Core.Notification.Providers;

namespace DevSAAS.Core.Notification.Services;

public class SmsService
{
    private const string Key = "sms_gateway";
    private readonly DatabaseFactory _databaseFactory;

    public SmsService(DatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<bool> SendAsync(string destination, string message)
    {
        var destinations = new[] { destination };
        return await SendAsync(destinations, message);
    }

    public async Task<bool> SendAsync(string[] destinations, string message)
    {
        await using var conn = _databaseFactory.Instance();

        try
        {
            var settingStore = new SettingStore(conn);
            var sms = await settingStore.GetByKeyAsync(Key);

            if (sms is null)
            {
                throw new Exception(LanguageService.Get("ConfigurationNotFound") + ": SMS");
            }

            var smsGateway = sms.Value;

            if (smsGateway.Equals("smsonlinegh", StringComparison.InvariantCultureIgnoreCase))
            {
                return await new SMSOnlineGH(settingStore).Send(destinations, message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }
}