using QueryLib.Dialects.Abstractions;

namespace QueryLib.Demo.Abstractions;

public interface IValueBinderFactory
{
    IValueBinder GetBinder(DbProvider binderType);
}