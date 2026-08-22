using System.Data;
using System.Data.Common;
using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Compilers;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Execution;
using QueryLib.Demo.Execution.Abstractions;

namespace QueryBuilder.Test.Execution;

public class DatabaseQueryExecutorTests
{
    private readonly IQueryExecutionDependencyFactory _dependencyFactory =
        Substitute.For<IQueryExecutionDependencyFactory>();
    private readonly DatabaseQueryExecutor _sut;

    public DatabaseQueryExecutorTests()
    {
        _sut = new DatabaseQueryExecutor(_dependencyFactory);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Arrange
        var configuration = new DbConfiguration(DbProvider.PostgreSql, "conn");

        // Act
        var act = () => _sut.ExecuteAsync(null!, configuration);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("query");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var query = new Query().From("student").Select("id");

        // Act
        var act = () => _sut.ExecuteAsync(query, null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("configuration");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldOpenConnectionAndReturnCombinedResult_WhenDependenciesSucceed()
    {
        // Arrange
        var query = new Query().From("student").Select("id");
        var configuration = new DbConfiguration(DbProvider.PostgreSql, "conn");
        var compiler = Substitute.For<ICompiler>();
        var runner = Substitute.For<IQueryRunner>();
        var connectionFactory = Substitute.For<IDbConnectionFactory>();
        var fakeConnection = new FakeDbConnection();
        var compiledQuery = new CompiledQuery("SELECT 1", []);
        var queryResult = new QueryResult { ColumnNames = [], Rows = [] };

        compiler.Compile(query).Returns(compiledQuery);
        connectionFactory.CreateConnectionAsync().Returns(Task.FromResult<DbConnection>(fakeConnection));
        runner.RunAsync(compiledQuery, fakeConnection, null).Returns(Task.FromResult(queryResult));
        _dependencyFactory.Create(configuration)
            .Returns(new QueryExecutionDependencies(compiler, runner, connectionFactory));
        var expected = new QueryExecutionResult(compiledQuery, queryResult);

        // Act
        var result = await _sut.ExecuteAsync(query, configuration);

        // Assert
        result.Should().BeEquivalentTo(expected);
        fakeConnection.WasOpened.Should().BeTrue();
    }

    private sealed class FakeDbConnection : DbConnection
    {
        public bool WasOpened { get; private set; }

        public override string ConnectionString { get; set; } = string.Empty;
        public override string Database => string.Empty;
        public override string DataSource => string.Empty;
        public override string ServerVersion => string.Empty;
        public override ConnectionState State => ConnectionState.Open;

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            WasOpened = true;
            return Task.CompletedTask;
        }

        public override void Open() => WasOpened = true;
        public override void Close() { }
        public override void ChangeDatabase(string databaseName) { }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
            throw new NotSupportedException();
        protected override DbCommand CreateDbCommand() => throw new NotSupportedException();
    }
}