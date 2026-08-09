using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo;

public record DbConfiguration(DbProvider provider, string connectionString);

public enum DbProvider
{
    
    PostgreSQL,
    Sqlserver
}

