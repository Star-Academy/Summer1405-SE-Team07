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

namespace QueryBuilder.Test.Execution;

public class DatabaseQueryExecutorTests
{
    private readonly IDbConnectionFactoryResolver _connectionFactoryResolver = Substitute.For<IDbConnectionFactoryResolver>();
    private readonly IQueryRunnerFactory _runnerFactory = Substitute.For<IQueryRunnerFactory>();
    private readonly ICompiler _compiler = Substitute.For<ICompiler>();
    private readonly DatabaseQueryExecutor _sut;

    public DatabaseQueryExecutorTests()
    {
        _sut = new DatabaseQueryExecutor(_connectionFactoryResolver, _runnerFactory, _compiler);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenResolverIsNull()
    {
        // Arrange
        var act = () => new DatabaseQueryExecutor(null!, _runnerFactory, _compiler);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("connectionFactoryResolver");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenRunnerFactoryIsNull()
    {
        // Arrange
        var act = () => new DatabaseQueryExecutor(_connectionFactoryResolver, null!, _compiler);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("runnerFactory");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenCompilerIsNull()
    {
        // Arrange
        var act = () => new DatabaseQueryExecutor(_connectionFactoryResolver, _runnerFactory, null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("compiler");
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
        var runner = Substitute.For<IQueryRunner>();
        var connectionFactory = Substitute.For<IDbConnectionFactory>();
        var fakeConnection = new FakeDbConnection();
        var compiledQuery = new CompiledQuery("SELECT 1", []);
        var queryResult = new QueryResult { ColumnNames = [], Rows = [] };

        _connectionFactoryResolver.GetFactory(DbProvider.PostgreSql).Returns(connectionFactory);
        _runnerFactory.GetRunner(DbProvider.PostgreSql).Returns(runner);
        connectionFactory.Create(configuration.ConnectionString).Returns(fakeConnection);
        _compiler.Compile(query, DbProvider.PostgreSql).Returns(compiledQuery);
        runner.RunAsync(compiledQuery, fakeConnection).Returns(Task.FromResult(queryResult));

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

        [System.Diagnostics.CodeAnalysis.AllowNull]
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