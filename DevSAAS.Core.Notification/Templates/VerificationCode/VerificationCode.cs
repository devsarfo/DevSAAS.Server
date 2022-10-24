using DevSAAS.Core.Helpers;
using DevSAAS.Core.Localization.Services;
using DevSAAS.Core.Notification.Services;

namespace DevSAAS.Core.Notification.Templates.VerificationCode;

public class VerificationCode
{
    private readonly SmsService? _smsService;
    private readonly MailService? _mailService;
    private readonly string _code, _phone, _email;

    public VerificationCode(SmsService smsService, MailService mailService, string code, string phone, string email)
    {
        _smsService = smsService;
        _mailService = mailService;
        _code = code;
        _phone = phone;
        _email = email;
    }

    public void Send()
    {
        if (_smsService != null)
        {
            var message = LanguageService.Get("VerificationSms").Replace(":otp", _code)
                .Replace(":title", LanguageService.Get("Title"));
            _smsService?.SendAsync(_phone, message);
        }

        if (_mailService == null) return;

        var email = EmbeddedResource.LoadResource("DevSAAS.Core.Notification.Templates.VerificationCode.mail.html");

        email = email.Replace("[type_of_action]", LanguageService.Get("VerificationCode"))
            .Replace("[app_title]", LanguageService.Get("Title"))
            .Replace("[confirm_account]", LanguageService.Get("ConfirmAccount"))
            .Replace("[confirm_account_hint]",
                LanguageService.Get("ConfirmAccountHint").Replace(":title", LanguageService.Get("Title")))
            .Replace("[mail_disclaimer]",
                LanguageService.Get("MailDisclaimer").Replace(":action", LanguageService.Get("VerificationCode")))
            .Replace("[code]", _code)
            .Replace("[regards]", LanguageService.Get("Regards"))
            .Replace("[action]", LanguageService.Get("VerifyAccount"))
            .Replace("[action_url]", "https://devsarfo.com");

        _mailService?.SendAsync(_email, LanguageService.Get("VerificationCode"), email);
    }
}