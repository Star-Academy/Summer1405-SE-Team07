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

        // Act
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("connectionFactoryResolver");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenRunnerFactoryIsNull()
    {
        // Arrange
        var act = () => new DatabaseQueryExecutor(_connectionFactoryResolver, null!, _compiler);

        // Act
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("runnerFactory");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenCompilerIsNull()
    {
        // Arrange
        var act = () => new DatabaseQueryExecutor(_connectionFactoryResolver, _runnerFactory, null!);

        // Act
        
        // Assert
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
}