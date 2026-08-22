using FluentAssertions;
using NSubstitute;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Renderers;
using QueryLib.Renderers.Abstractions;

namespace QueryBuilder.Test.Renderers;

public class ClauseRendererRegistryTests
{
    [Fact]
    public void GetRenderer_ShouldReturnMatchingRenderer_WhenRegisteredForClauseType()
    {
        // Arrange
        var selectRenderer = Substitute.For<IClauseRenderer>();
        selectRenderer.ClauseType.Returns(typeof(SelectClause));
        var sut = new ClauseRendererRegistry([selectRenderer]);
        var clause = new SelectClause { Columns = [] };

        // Act
        var result = sut.GetRenderer(clause);

        // Assert
        result.Should().BeSameAs(selectRenderer);
    }

    [Fact]
    public void GetRenderer_ShouldThrowInvalidOperationException_WhenNoRendererRegisteredForClauseType()
    {
        // Arrange
        var sut = new ClauseRendererRegistry([]);
        var clause = new FromClause();

        // Act
        var act = () => sut.GetRenderer(clause);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No renderer registered for FromClause.");
    }
}