using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using QueryLib.Compilers;
using QueryLib.Compilers.Abstractions;

namespace QueryBuilder.Test.Compilers;

public class SqlCompilerFactoryTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenProviderIsNull()
    {
        // Arrange

        // Act
        var act = () => new SqlCompilerFactory(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("provider");
    }

    [Fact]
    public void Create_ShouldReturnKeyedCompiler_WhenKeyIsRegistered()
    {
        // Arrange
        var expectedCompiler = Substitute.For<ICompiler>();
        var services = new ServiceCollection();
        services.AddKeyedSingleton("postgres", expectedCompiler);
        var provider = services.BuildServiceProvider();
        var sut = new SqlCompilerFactory(provider);

        // Act
        var result = sut.Create("postgres");

        // Assert
        result.Should().BeSameAs(expectedCompiler);
    }
}