namespace DevSAAS.Core.Notification.Interfaces;

public interface IMailProvider
{
    Task<bool> Send(string[] destinations, string subject, string message);
}