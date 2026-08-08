namespace QueryLib.Dialects;


public sealed class PassthroughValueBinder : IValueBinder
{
    public object? Bind(object? value) => value;
}
