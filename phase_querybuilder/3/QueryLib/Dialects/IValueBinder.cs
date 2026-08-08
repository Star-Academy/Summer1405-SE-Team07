
namespace QueryLib.Dialects ;


public interface IValueBinder
{
    object? Bind(object? value);
}
