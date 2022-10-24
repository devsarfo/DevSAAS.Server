using DevSAAS.Core.Helpers;
using DevSAAS.Core.Localization.Services;
using DevSAAS.Core.Notification.Services;

namespace DevSAAS.Core.Notification.Templates.VerificationMail;

public class VerificationMail
{
    private readonly MailService? _mailService;
    private readonly string _url, _email;

    public VerificationMail(MailService mailService, string email, string url)
    {
        _mailService = mailService;
        _url = url;
        _email = email;
    }

    public void Send()
    {
        if (_mailService == null) return;

        var email = EmbeddedResource.LoadResource("DevSAAS.Core.Notification.Templates.VerificationMail.mail.html");

        email = email.Replace("[type_of_action]", LanguageService.Get("VerificationCode"))
            .Replace("[app_title]", LanguageService.Get("Title"))
            .Replace("[confirm_email_address]", LanguageService.Get("ConfirmYourEmailAddress"))
            .Replace("[confirm_email_address_hint]",
                LanguageService.Get("ConfirmYourEmailAddressHint").Replace(":title", LanguageService.Get("Title")))
            .Replace("[mail_disclaimer]",
                LanguageService.Get("MailDisclaimer").Replace(":action", LanguageService.Get("VerificationCode")))
            .Replace("[confirm_email_address_open_url]", LanguageService.Get("ConfirmYourEmailAddressOpenUrl"))
            .Replace("[regards]", LanguageService.Get("Regards"))
            .Replace("[action]", LanguageService.Get("VerifyAccount"))
            .Replace("[action_url]", _url);

        _mailService?.SendAsync(_email, LanguageService.Get("VerificationCode"), email);
    }
}