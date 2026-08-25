using FluentAssertions;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;

namespace QueryBuilder.Test.Clauses;

public class SelectClauseTests
{
    [Fact]
    public void Columns_ShouldReturnProvidedColumns_WhenSet()
    {
        // Arrange
        var expected = new[] { "id", "name" };
        var sut = new SelectClause { Columns = expected };

        // Act
        var columns = sut.Columns;

        // Assert
        columns.Should().Equal(expected);
    }

    [Fact]
    public void Order_ShouldBe0()
    {
        // Arrange
        const int expected = 0;
        var sut = new SelectClause { Columns = [] };

        // Act
        var order = sut.Order;

        // Assert
        order.Should().Be(expected);
    }

    [Fact]
    public void Kind_ShouldBeSelect()
    {
        // Arrange
        const ClauseKind expected = ClauseKind.Select;
        var sut = new SelectClause { Columns = [] };

        // Act
        var kind = sut.Kind;

        // Assert
        kind.Should().Be(expected);
    }
}