using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DevSAAS.Core.Database;

public sealed class DatabaseFactory
{
    private readonly IConfiguration _configuration;

    public DatabaseFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbConnection Instance()
    {
        var connStr = _configuration.GetConnectionString("Postgres");
        return new NpgsqlConnection(connStr);
    }
}