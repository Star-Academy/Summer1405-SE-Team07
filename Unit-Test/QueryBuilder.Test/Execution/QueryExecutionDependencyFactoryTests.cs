using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Execution;

namespace QueryBuilder.Test.Execution;

public class QueryExecutionDependencyFactoryTests
{
    private readonly ISqlCompilerFactory _compilerFactory = Substitute.For<ISqlCompilerFactory>();

    [Fact]
    public void Create_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var sut = new QueryExecutionDependencyFactory(_compilerFactory, services.BuildServiceProvider());

        // Act
        var act = () => sut.Create(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("configuration");
    }

    [Fact]
    public void Create_ShouldResolvePostgresDependencies_WhenProviderIsPostgreSql()
    {
        // Arrange
        var expectedCompiler = Substitute.For<ICompiler>();
        var expectedRunner = Substitute.For<IQueryRunner>();
        var expectedConnectionFactory = Substitute.For<IDbConnectionFactory>();
        var services = new ServiceCollection();
        services.AddKeyedSingleton("postgres", expectedRunner);
        services.AddKeyedSingleton<Func<string, IDbConnectionFactory>>(
            "postgres", (_, _) => _ => expectedConnectionFactory);
        var sut = new QueryExecutionDependencyFactory(_compilerFactory, services.BuildServiceProvider());
        var configuration = new DbConfiguration(DbProvider.PostgreSql, "conn-string");
        _compilerFactory.Create("postgres").Returns(expectedCompiler);
        var expected = new QueryExecutionDependencies(expectedCompiler, expectedRunner, expectedConnectionFactory);

        // Act
        var result = sut.Create(configuration);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Create_ShouldThrowArgumentOutOfRangeException_WhenProviderIsUnsupported()
    {
        // Arrange
        var services = new ServiceCollection();
        var sut = new QueryExecutionDependencyFactory(_compilerFactory, services.BuildServiceProvider());
        var configuration = new DbConfiguration((DbProvider)999, "conn-string");

        // Act
        var act = () => sut.Create(configuration);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("Provider");
    }
}