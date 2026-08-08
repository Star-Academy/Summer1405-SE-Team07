namespace QueryLib.Dialects.Postgres;

public sealed class PostgresParameterPlaceholderFactory : IParameterPlaceholderFactory
{
    public string MakePlaceholder(int index) => $"${index}";
}
