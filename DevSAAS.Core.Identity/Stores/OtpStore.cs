using System.Data;
using Dapper;
using DevSAAS.Core.Helpers;
using DevSAAS.Core.Identity.Entities;
using DevSAAS.Core.Stores;

namespace DevSAAS.Core.Identity.Stores;

public class OtpStore: Store<Otp>
{
    public OtpStore(IDbConnection conn) : base(conn, "otps") { }
    
    public Task<Otp?> GetByUserIdCodeAsync(string userId, string code)
    {
        const string statement = @"
                select *
                from public.otps o
                where o.user_id = @_user_id and o.code = @_code;
            ";

        return Connection.QueryFirstOrDefaultAsync<Otp?>(statement, new { _user_id = userId.Trim(), _code = code.Trim() });
    }
    
    public Task<IEnumerable<Otp>> GetByUserIdAsync(string userId)
    {
        const string statement = @"
                select *
                from public.otps o
                where o.user_id = @_user_id;
            ";

        return Connection.QueryFirstOrDefaultAsync<IEnumerable<Otp>>(statement, new { _user_id = userId.Trim() });
    }

    public async Task<string> GenerateCode(string userId)
    {
        var code = Helper.RandomNumbers(4);
        await InsertAsync(new Otp(userId, code));

        return code;
    }
}