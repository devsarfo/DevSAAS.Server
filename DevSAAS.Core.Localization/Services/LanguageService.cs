using DevSAAS.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevSAAS.Core.Localization.Services;

public static class LanguageService
{
    private static readonly Dictionary<string, JObject> Languages = new();

    static LanguageService()
    {
        var str = EmbeddedResource.LoadResource("DevSAAS.Core.Localization.Languages.en.json");
        var lang = JsonConvert.DeserializeObject<JObject>(str);
        Languages.Add("en", lang!);
    }

    public static string Get(string key, string lang = "en")
    {
        return Languages[lang][key]?.ToString() ?? key;
    }
    
}