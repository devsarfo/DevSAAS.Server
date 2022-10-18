using DevSAAS.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevSAAS.Core.Localization.Services;

public sealed class LanguageService
{
    private readonly string _language;
    private string _languages;

    public LanguageService(string language = "en")
    {
        _language = language;
        Initialize().Wait();
    }
    
    private async Task Initialize()
    {
        _languages = await EmbeddedResource.LoadResourceAsync("DevSAAS.Core.Localization.Languages."+_language+".json");
    }

    public string Get(string key)
    {
        var lang = (JObject) JsonConvert.DeserializeObject(_languages);
        return lang[key].Value<string>() ?? key;
    }
}