using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Entities;
using DevSAAS.Core.Identity.Stores;

namespace DevSAAS.Core.Identity.Services;

public class RoleService
{
    private readonly DatabaseFactory _databaseFactory;

    public RoleService(DatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }
}