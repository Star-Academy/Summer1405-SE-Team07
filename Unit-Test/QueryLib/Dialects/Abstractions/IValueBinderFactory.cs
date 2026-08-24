using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.Abstractions;

public interface IValueBinderFactory
{
    IValueBinder GetBinder(DbProvider binderType);
}