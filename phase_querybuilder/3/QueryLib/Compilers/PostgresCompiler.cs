namespace QueryLib.Compilers
{
public class PostgresCompiler : CompilerBase
{
    protected override string QuoteIdentifier(string identifier) => $"\"{identifier}\"";

    protected override string CreatePlaceholder(int index) => "$" + (index + 1);
}
}