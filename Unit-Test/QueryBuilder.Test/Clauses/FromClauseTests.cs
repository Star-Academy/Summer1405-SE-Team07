using FluentAssertions;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;

namespace QueryBuilder.Test.Clauses;

public class FromClauseTests
{
    private readonly FromClause _sut;

    public FromClauseTests()
    {
        _sut = new FromClause { Table = "student" };
    }

    [Fact]
    public void Table_ShouldReturnSetValue_WhenConstructed()
    {
        // Arrange
        const string expected = "student";

        // Act
        var table = _sut.Table;

        // Assert
        table.Should().Be(expected);
    }

    [Fact]
    public void Order_ShouldBe1()
    {
        // Arrange
        const int expected = 1;

        // Act
        var order = _sut.Order;

        // Assert
        order.Should().Be(expected);
    }

    [Fact]
    public void Kind_ShouldBeFrom()
    {
        // Arrange
        const ClauseKind expected = ClauseKind.From;

        // Act
        var kind = _sut.Kind;

        // Assert
        kind.Should().Be(expected);
    }
}