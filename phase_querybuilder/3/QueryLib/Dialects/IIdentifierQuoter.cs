namespace QueryLib.Dialects ;


public interface IIdentifierQuoter
{
    string Quote(string identifier);
}
