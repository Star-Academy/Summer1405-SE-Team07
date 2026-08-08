

namespace QueryLib.Dialects ;


public interface IParameterPlaceholderFactory
{
    string MakePlaceholder(int index);
}
