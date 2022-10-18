using System.Data;
using System.Data.Common;
using Dapper;
using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Entities;
using DevSAAS.Core.Stores;

namespace DevSAAS.Core.Identity.Stores;

public sealed class UserStore : Store<User>
{
    public UserStore(IDbConnection conn) : base(conn, "users")
    {
        
    }
    
    public Task<User?> GetByUsernameAsync(string username)
    {
        const string statement = @"
                select u.*
                from public.users u
                where u.email = @_username or u.phone = @_username;
            ";

        return Connection.QueryFirstOrDefaultAsync<User?>(statement, new { _username = username.Trim() });
    }
    
    public Task<User?> GetByEmailAsync(string value)
    {
        const string statement = @"
                select u.*
                from public.users u
                where u.email = @_value;
            ";

        return Connection.QueryFirstOrDefaultAsync<User?>(statement, new { _value = value.Trim() });
    }
    
    public Task<User?> GetByPhoneAsync(string value)
    {
        const string statement = @"
                select u.*
                from public.users u
                where u.phone = @_value;
            ";

        return Connection.QueryFirstOrDefaultAsync<User?>(statement, new { _value = value.Trim() });
    }
}