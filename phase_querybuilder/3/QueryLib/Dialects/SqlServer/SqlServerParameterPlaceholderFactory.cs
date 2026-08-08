using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.SqlServer;

public sealed class SqlServerParameterPlaceholderFactory : IParameterPlaceholderFactory
{
    public string MakePlaceholder(int index) => $"@p{index - 1}";
}
