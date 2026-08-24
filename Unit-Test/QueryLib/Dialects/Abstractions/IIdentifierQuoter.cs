namespace QueryLib.Dialects.Abstractions;

public interface IIdentifierQuoter
{
    string Quote(string identifier);
    DbProvider Provider { get; }
}
