namespace asp_webapi.Exceptions;

public class UnsupportedDatabaseTypeException : Exception
{
    public UnsupportedDatabaseTypeException(string? dbType)
        : base($"Database type '{dbType}' is not supported.") { }
}