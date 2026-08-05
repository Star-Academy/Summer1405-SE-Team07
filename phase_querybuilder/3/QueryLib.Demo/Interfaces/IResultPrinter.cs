using System.Data.Common;

namespace QueryLib.Demo
{
    public interface IResultPrinter
    {
        Task PrintAsync(DbDataReader reader, string header);
    }

}