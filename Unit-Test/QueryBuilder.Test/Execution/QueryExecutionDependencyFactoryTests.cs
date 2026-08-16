using FluentAssertions;
using NSubstitute;
using QueryLib.Compilers;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo;
using QueryLib.Demo.Connections;
using QueryLib.Demo.Execution;
using QueryLib.Demo.Execution.Abstractions;
using QueryLib.Demo.QueryRunners;

namespace QueryBuilder.Test.Execution;

public class QueryExecutionDependencyFactoryTests
{
    private readonly IQueryExecutionDependencyFactory _sut;
    private readonly ISqlCompilerFactory _compilerFactory;

    public QueryExecutionDependencyFactoryTests()
    {
        _compilerFactory = Substitute.For<ISqlCompilerFactory>();
        _sut = new QueryExecutionDependencyFactory(_compilerFactory);
    }

    [Fact]
    public void Create_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        DbConfiguration? configuration = null;

        // Act
        var act = () => _sut.Create(configuration!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("configuration");
    }

    [Fact]
    public void Create_ShouldReturnPostgresDependencies_WhenProviderIsPostgreSql()
    {
        // Arrange
        var configuration = new DbConfiguration(DbProvider.PostgreSql, "connection-string");
        var compiler = Substitute.For<ICompiler>();
        _compilerFactory.Create("postgres").Returns(compiler);

        // Act
        var result = _sut.Create(configuration);

        // Assert
        result.Compiler.Should().BeSameAs(compiler);
        result.Runner.Should().BeOfType<PostgresQueryRunner>();
        result.ConnectionFactory.Should().BeOfType<PostgresConnectionFactory>();
        _compilerFactory.Received(1).Create("postgres");
    }

    [Fact]
    public void Create_ShouldReturnSqlServerDependencies_WhenProviderIsSqlServer()
    {
        // Arrange
        var configuration = new DbConfiguration(DbProvider.SqlServer, "connection-string");
        var compiler = Substitute.For<ICompiler>();
        _compilerFactory.Create("sqlserver").Returns(compiler);

        // Act
        var result = _sut.Create(configuration);

        // Assert
        result.Compiler.Should().BeSameAs(compiler);
        result.Runner.Should().BeOfType<SqlServerQueryRunner>();
        result.ConnectionFactory.Should().BeOfType<SqlServerConnectionFactory>();
        _compilerFactory.Received(1).Create("sqlserver");
    }

    [Fact]
    public void Create_ShouldThrowArgumentOutOfRangeException_WhenProviderIsUnsupported()
    {
        // Arrange
        var provider = (DbProvider)999;
        var configuration = new DbConfiguration(provider, "connection-string");

        // Act
        var act = () => _sut.Create(configuration);

        // Assert
        act.Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithParameterName("Provider")
            .Which.ActualValue.Should().Be(provider);
    }
}
