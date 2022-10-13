using DevSAAS.Core.Database;
using DevSAAS.Core.Identity.Entities;
using DevSAAS.Core.Identity.Stores;

namespace DevSAAS.Core.Identity.Services;

public class UserService
{
    private readonly DatabaseFactory _DatabaseFactory;

    public UserService(DatabaseFactory DatabaseFactory)
    {
        _DatabaseFactory = DatabaseFactory;
    }

    public async Task<User?> Login(string username, string password)
    {
        await using var conn = _DatabaseFactory.Instance();
        var userStore = new UserStore(conn);
        var user = await userStore.GetByUsernameAsync(username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            throw new ApplicationException("Invalid Username or Password");
        }
        
        return user;
    }
}