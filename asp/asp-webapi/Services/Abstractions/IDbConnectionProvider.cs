using System.Data;
using SqlKata.Compilers;

namespace asp_webapi.Services.Abstractions;

public interface IDbConnectionProvider
{
    IDbConnection CreateConnection();
    Compiler CreateCompiler();
}