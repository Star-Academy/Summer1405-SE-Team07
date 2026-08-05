namespace QueryLib.Compilers;

public class SqlServerCompiler : CompilerBase
{
    protected override string QuoteIdentifier(string identifier) => $"[{identifier}]";

    protected override string CreatePlaceholder(int index) => "@p" + index;

    protected override object? PrepareBinding(object? value)
    {
        if (value is bool b)
        {
            return b ? 1 : 0;
                
        }

        return value;
    }
}

