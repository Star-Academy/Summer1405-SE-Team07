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
    public void Constructor_ShouldThrowArgumentNullException_WhenDependencyFactoryIsNull()
    {
        // Arrange
        var expected = "dependencyFactory";

        // Act
        var act = () => new DatabaseQueryExecutor(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName(expected);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Arrange
        var configuration = new DbConfiguration(DbProvider.PostgreSql, "connectionString");
        var expected = "query";

        // Act
        var act = () => _sut.ExecuteAsync(null!, configuration);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName(expected);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var query = new Query();
        var expected = "configuration";

        // Act
        var act = () => _sut.ExecuteAsync(query, null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName(expected);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCompileOpenExecuteAndReturnResult_WhenInputsAreValid()
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
        var expected = new QueryExecutionResult(compiledQuery, queryResult);

        // Act
        var result = await _sut.ExecuteAsync(query, configuration);

        // Assert
        result.Should().BeEquivalentTo(expected);

        await connection.Received(1).OpenAsync();
        await runner.Received(1).RunAsync(compiledQuery, connection);
    }
}

