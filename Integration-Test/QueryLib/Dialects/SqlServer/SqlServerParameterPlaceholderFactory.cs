using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.SqlServer;

public sealed class SqlServerParameterPlaceholderFactory : IParameterPlaceholderFactory
{
    public DbProvider Provider => DbProvider.SqlServer;
    public string MakePlaceholder(int index) => $"@p{index - 1}";
}
