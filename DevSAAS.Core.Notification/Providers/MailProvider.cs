using System.Net;
using System.Net.Mail;
using DevSAAS.Core.Configuration.Stores;
using DevSAAS.Core.Localization.Services;
using DevSAAS.Core.Notification.Interfaces;

namespace DevSAAS.Core.Notification.Providers;

public class MailProvider : IMailProvider
{
    private readonly string _key;
    private readonly SettingStore _settingStore;

    public MailProvider(SettingStore settingStore, string key)
    {
        _settingStore = settingStore;
        _key = key;
    }

    public async Task<bool> Send(string[] destinations, string subject, string message)
    {
        var title = await _settingStore.GetByKeyAsync("title");
        var config = await _settingStore.GetByAllKeyAsync(_key);
        if (config == null)
        {
            throw new Exception(LanguageService.Get("ConfigurationNotFound") + ": Gmail");
        }

        var senderName = title is not null ? title.Value : LanguageService.Get("Title");
        var smtpUrl = config.First(c => c.Key == _key + "_smtp_url").Value;
        var smtpPort = config.First(c => c.Key == _key + "_smtp_port").Value;
        var senderEmail = config.First(c => c.Key == _key + "_smtp_sender").Value;
        var smtpUsername = config.First(c => c.Key == _key + "_smtp_username").Value;
        var smtpPassword = config.First(c => c.Key == _key + "_smtp_password").Value;
        var smtpSsl = config.First(c => c.Key == _key + "_smtp_ssl").Value;

        var smtpClient = new SmtpClient
        {
            Host = smtpUrl,
            Port = int.Parse(smtpPort),
            EnableSsl = bool.Parse(smtpSsl),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(smtpUsername, smtpPassword)
        };

        var mailMessage = new MailMessage();
        foreach (var destination in destinations)
        {
            mailMessage.To.Add(new MailAddress(destination));
        }

        mailMessage.Subject = subject;
        mailMessage.From = new MailAddress(senderEmail, senderName);
        mailMessage.Body = message;
        mailMessage.IsBodyHtml = true;
        smtpClient.SendAsync(mailMessage, null);

        //TODO: Handle results

        return true;
    }
}