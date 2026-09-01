using System.Data;
using Microsoft.Data.SqlClient;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace asp_webapi.Services;

public class DatabaseFactory : IDatabaseFactory
{
    private readonly IConfiguration _configuration;

    public DatabaseFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public QueryFactory CreateQueryFactory(string? dbType)
    {
        if (string.IsNullOrWhiteSpace(dbType))
        {
            throw new ArgumentException("Database type not provided.");
        }

        dbType = dbType.Trim().ToLowerInvariant();

        switch (dbType)
        {
            case "sqlserver":
            {
                var connectionString = _configuration.GetConnectionString("SqlServerConnection");
                IDbConnection connection = new SqlConnection(connectionString);
                var compiler = new SqlServerCompiler();
                return new QueryFactory(connection, compiler);
            }
            case "postgres":
            {
                var connectionString = _configuration.GetConnectionString("PostgresConnection");
                IDbConnection connection = new NpgsqlConnection(connectionString);
                var compiler = new PostgresCompiler();
                return new QueryFactory(connection, compiler);
            }
            default:
                throw new ArgumentException($"Database type {dbType} is not supported.");
        }
    }
}