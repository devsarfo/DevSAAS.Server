using System.Net.Http.Headers;
using DevSAAS.Core.Configuration.Stores;
using DevSAAS.Core.Localization.Services;
using DevSAAS.Core.Notification.Interfaces;

namespace DevSAAS.Core.Notification.Providers;

public class SmsOnlineGh : ISmsProvider
{
    private const string Key = "smsonlinegh";
    private readonly SettingStore _settingStore;

    public SmsOnlineGh(SettingStore settingStore)
    {
        _settingStore = settingStore;
    }


    public async Task<bool> Send(string[] destinations, string message)
    {
        var config = await _settingStore.GetByAllKeyAsync(Key);
        if (config == null)
        {
            throw new Exception(LanguageService.Get("ConfigurationNotFound") + ": SMSOnlineGH");
        }

        var senderId = config.First(c => c.Key == Key + "_sender_id").Value;
        var apiKey = config.First(c => c.Key == Key + "_api_key").Value;
        var baseUrl = config.First(c => c.Key == Key + "_base_url").Value;

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("Authorization", "key " + apiKey);

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("key", apiKey),
            new("text", message),
            new("type", "0"),
            new("sender", senderId),
            new("to", string.Join(",", destinations)),
        };

        var request = new HttpRequestMessage(HttpMethod.Post, baseUrl)
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(LanguageService.Get("SendError").Replace(":object", "SMSOnlineGH"));
        }

        //TODO: Handle results
        // Console.WriteLine(await response.Content.ReadAsStringAsync());

        return true;
    }
}