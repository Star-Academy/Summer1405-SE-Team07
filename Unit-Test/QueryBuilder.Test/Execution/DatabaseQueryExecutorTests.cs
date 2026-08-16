using QueryLib.Demo.Execution.Abstractions;
using QueryLib.Demo.Execution;
using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Demo;
using QueryLib.Demo.Abstractions;
using System.Data.Common;
using QueryLib.Compilers;
using QueryLib.Compilers.Abstractions;

namespace QueryBuilder.Test.Execution;

public class DatabaseQueryExecutorTests
{
    private readonly IQueryExecutionDependencyFactory _dependencyFactory;
    private readonly DatabaseQueryExecutor _sut;

    public DatabaseQueryExecutorTests()
    {
        _dependencyFactory = Substitute.For<IQueryExecutionDependencyFactory>();
        _sut = new DatabaseQueryExecutor(_dependencyFactory);
    }

    [Fact]
    public void Constructor_WhenDependencyFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IQueryExecutionDependencyFactory? dependencyFactory = null;

        // Act
        var act = () => new DatabaseQueryExecutor(dependencyFactory!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("dependencyFactory");
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var configuration = new DbConfiguration(DbProvider.PostgreSql, "connectionString");

        // Act
        var act = () => _sut.ExecuteAsync(null!, configuration);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("query");
    }

    [Fact]
    public async Task ExecuteAsync_WhenConfigurationIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var query = new Query();

        // Act
        var act = () => _sut.ExecuteAsync(query, null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("configuration");
    }

    [Fact]
    public async Task ExecuteAsync_WhenInputsAreValid_ShouldCompileOpenExecuteAndReturnResult()
    {
        // Arrange
        var query = new Query().From("student");

        var configuration = new DbConfiguration(
            DbProvider.PostgreSql,
            "test-connection-string");

        var compiler = Substitute.For<ICompiler>();
        var connectionFactory = Substitute.For<IDbConnectionFactory>();
        var runner = Substitute.For<IQueryRunner>();
        var connection = Substitute.For<DbConnection>();

        var compiledQuery = new CompiledQuery(
            "SELECT * FROM student",
            Array.Empty<object?>());

        var queryResult = new QueryResult();

        var dependencies = new QueryExecutionDependencies(
            compiler,
            runner,
            connectionFactory);

        _dependencyFactory.Create(configuration).Returns(dependencies);
        compiler.Compile(query).Returns(compiledQuery);
        connectionFactory.CreateConnectionAsync().Returns(connection);
        runner.RunAsync(compiledQuery, connection).Returns(queryResult);

        // Act
        var result = await _sut.ExecuteAsync(query, configuration);

        // Assert
        result.Should().NotBeNull();
        result.CompiledQuery.Should().Be(compiledQuery);
        result.QueryResult.Should().Be(queryResult);
        _dependencyFactory.Received(1).Create(configuration);
        compiler.Received(1).Compile(query);
        await connectionFactory.Received(1).CreateConnectionAsync();
        await connection.Received(1).OpenAsync();
        await runner.Received(1).RunAsync(compiledQuery, connection);
    }
}

