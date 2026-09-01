using System.Data;
using Microsoft.Data.SqlClient;
using SqlKata.Compilers;
using asp_webapi.Services.Abstractions;

namespace asp_webapi.Services;

public class SqlServerConnectionProvider : IDbConnectionProvider
{
    private readonly IConfiguration _configuration;

    public SqlServerConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection() =>
        new SqlConnection(_configuration.GetConnectionString("SqlServerConnection"));

    public Compiler CreateCompiler() => new SqlServerCompiler();
}