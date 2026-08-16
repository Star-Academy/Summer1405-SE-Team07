
namespace QueryLib.Dialects.Abstractions;


public interface IValueBinder
{
    object? Bind(object? value);
}
