using DevSAAS.Core.Configuration.Stores;

namespace DevSAAS.Core.Notification.Interfaces;

public interface ISmsProvider
{
    Task<bool> Send(string[] destinations, string message);
}