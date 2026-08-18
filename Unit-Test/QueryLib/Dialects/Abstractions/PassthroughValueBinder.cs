namespace QueryLib.Dialects.Abstractions;

public sealed class PassthroughValueBinder : IValueBinder
{
    public object? Bind(object? value) => value;
}
