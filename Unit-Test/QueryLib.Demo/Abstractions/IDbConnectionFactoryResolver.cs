namespace QueryLib.Demo.Abstractions;

public interface IDbConnectionFactoryResolver
{
    IDbConnectionFactory GetFactory(DbProvider provider);
}

