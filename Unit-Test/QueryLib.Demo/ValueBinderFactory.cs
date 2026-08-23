using QueryLib.Demo.Abstractions;
using QueryLib.Dialects.Abstractions;

namespace QueryLib.Demo;

public class ValueBinderFactory: IValueBinderFactory
{
    private readonly Dictionary<DbProvider, IValueBinder> _binders;

    public ValueBinderFactory(IEnumerable<IValueBinder> binders)
    {
        _binders =  binders.ToDictionary(x => x.DbType);
    }
    public IValueBinder GetBinder(DbProvider binderType)
    {
        if (_binders.TryGetValue(binderType, out var binder))
        {
            return binder;
        }
        throw new KeyNotFoundException($"No binder registered for type {binderType}");
    }
}