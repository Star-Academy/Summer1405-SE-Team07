using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Renderers;
using QueryLib.Renderers.Abstractions;

namespace QueryBuilder.Test.Renderers;

public class ClauseRendererRegistryTests
{
    [Fact]
    public void Provider_ShouldReturnConfiguredProvider_WhenConstructed()
    {
        // Arrange
        const DbProvider expected = DbProvider.PostgreSql;
        var sut = new ClauseRendererRegistry(expected, []);

        // Act
        var provider = sut.Provider;

        // Assert
        provider.Should().Be(expected);
    }

    [Fact]
    public void GetRenderer_ShouldReturnMatchingRenderer_WhenRegisteredForClauseKind()
    {
        // Arrange
        var selectRenderer = Substitute.For<IClauseRenderer>();
        selectRenderer.ClauseKind.Returns(ClauseKind.Select);
        var sut = new ClauseRendererRegistry(DbProvider.PostgreSql, [selectRenderer]);
        var clause = new SelectClause { Columns = [] };

        // Act
        var result = sut.GetRenderer(clause);

        // Assert
        result.Should().BeSameAs(selectRenderer);
    }

    [Fact]
    public void GetRenderer_ShouldThrowInvalidOperationException_WhenNoRendererRegisteredForClauseKind()
    {
        // Arrange
        var sut = new ClauseRendererRegistry(DbProvider.PostgreSql, []);
        var clause = new FromClause { Table = "student" };

        // Act
        var act = () => sut.GetRenderer(clause);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No renderer registered for From.");
    }
}