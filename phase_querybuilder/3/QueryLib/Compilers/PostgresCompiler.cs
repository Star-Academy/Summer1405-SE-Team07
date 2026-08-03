namespace QueryLib.Compilers
{
    // SRP: this class contains only PostgreSQL-specific compiler behavior.
    // LSP: it can replace any ICompiler/CompilerBase without changing caller behavior.
    public class PostgresCompiler : CompilerBase
    {
        protected override string QuoteIdentifier(string identifier) => $"\"{identifier}\"";

        protected override string CreatePlaceholder(int index) => "$" + (index + 1);
    }
}


