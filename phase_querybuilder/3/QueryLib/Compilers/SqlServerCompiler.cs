namespace QueryLib.Compilers
{
    // SRP: this class contains only SQL Server-specific compiler behavior.
    // LSP: it can replace any ICompiler/CompilerBase without changing caller behavior.
    public class SqlServerCompiler : CompilerBase
    {
        protected override string QuoteIdentifier(string identifier) => $"[{identifier}]";

        protected override string CreatePlaceholder(int index) => "@p" + index;

        protected override object? PrepareBinding(object? value)
        {
            if (value is bool b)
                return b ? 1 : 0;

            return value;
        }
    }
}
