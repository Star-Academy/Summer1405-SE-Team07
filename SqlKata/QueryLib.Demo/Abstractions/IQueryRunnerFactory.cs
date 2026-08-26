namespace QueryLib.Demo.Abstractions;

public interface IQueryRunnerFactory
{
    IQueryRunner GetRunner(DbProvider provider);
}

