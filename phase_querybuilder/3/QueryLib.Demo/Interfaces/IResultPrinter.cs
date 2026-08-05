using System.Data.Common;

namespace QueryLib.Demo;

public interface IResultPrinter
{
    Task PrintAsync(QueryResult result, string header);
}
