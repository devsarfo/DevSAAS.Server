using System.Data;
using Dapper;
using DevSAAS.Core.Configuration.Entities;
using DevSAAS.Core.Database;

namespace DevSAAS.Core.Configuration.Stores;

public class SettingStore : Store<Setting>
{
    public SettingStore(IDbConnection conn) : base(conn, "settings")
    {
    }

    public Task<Setting?> GetByKeyAsync(string key)
    {
        const string statement = @"
                select *
                from public.settings s
                where s.key = @_key;
            ";

        return Connection.QueryFirstOrDefaultAsync<Setting?>(statement, new { _key = key.Trim() });
    }

    public Task<IEnumerable<Setting>> GetByAllKeyAsync(string key)
    {
        const string statement = @"
                select *
                from public.settings s
                where s.key like @_key;
            ";

        return Connection.QueryAsync<Setting>(statement, new { _key = key.Trim() + "_%" });
    }
}