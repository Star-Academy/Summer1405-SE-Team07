using System.Data;
using Npgsql;
using SqlKata.Compilers;
using asp_webapi.Services.Abstractions;

namespace asp_webapi.Services;

public class PostgresConnectionProvider : IDbConnectionProvider
{
    private readonly IConfiguration _configuration;

    public PostgresConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection() =>
        new NpgsqlConnection(_configuration.GetConnectionString("PostgresConnection"));

    public Compiler CreateCompiler() => new PostgresCompiler();
}