using DevSAAS.Core.Database;

namespace DevSAAS.Core.Identity.Services;

public class RoleService
{
    private readonly DatabaseFactory _databaseFactory;

    public RoleService(DatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }
}