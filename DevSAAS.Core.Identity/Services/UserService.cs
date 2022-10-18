using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Entities;
using DevSAAS.Core.Identity.Stores;

namespace DevSAAS.Core.Identity.Services;

public class UserService
{
    private readonly DatabaseFactory _databaseFactory;

    public UserService(DatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }
    
}