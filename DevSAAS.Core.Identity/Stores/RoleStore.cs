using System.Data;
using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Entities;

namespace DevSAAS.Core.Identity.Stores;

public class RoleStore : Store<Role>
{
    public RoleStore(IDbConnection conn) : base(conn, "roles")
    {
    }
}