using DevSAAS.Core.Localization.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevSAAS.Web.Controllers;

public class ApiController : ControllerBase
{
    private readonly LanguageService _languageService;

    public ApiController()
    {
        
    }
    
    public ApiController(LanguageService _languageService) : this()
    {
        _languageService = _languageService;
    }
}