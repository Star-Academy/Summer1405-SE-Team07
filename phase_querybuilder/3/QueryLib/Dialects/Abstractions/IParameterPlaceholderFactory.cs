

namespace QueryLib.Dialects.Abstractions;


public interface IParameterPlaceholderFactory
{
    string MakePlaceholder(int index);
}


