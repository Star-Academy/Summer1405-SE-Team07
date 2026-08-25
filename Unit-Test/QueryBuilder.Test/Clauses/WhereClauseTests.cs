using FluentAssertions;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;

namespace QueryBuilder.Test.Clauses;

public class WhereClauseTests
{
    [Fact]
    public void HasConditions_ShouldBeFalse_WhenConditionsIsEmpty()
    {
        // Arrange
        var sut = new WhereClause { Conditions = [] };

        // Act
        var hasConditions = sut.HasConditions;

        // Assert
        hasConditions.Should().BeFalse();
    }

    [Fact]
    public void HasConditions_ShouldBeTrue_WhenConditionsIsNotEmpty()
    {
        // Arrange
        var sut = new WhereClause
        {
            Conditions = [new Condition { Column = "name", Value = "kourosh" }]
        };

        // Act
        var hasConditions = sut.HasConditions;

        // Assert
        hasConditions.Should().BeTrue();
    }

    [Fact]
    public void Order_ShouldBe2()
    {
        // Arrange
        const int expected = 2;
        var sut = new WhereClause { Conditions = [] };

        // Act
        var order = sut.Order;

        // Assert
        order.Should().Be(expected);
    }

    [Fact]
    public void Kind_ShouldBeWhere()
    {
        // Arrange
        const ClauseKind expected = ClauseKind.Where;
        var sut = new WhereClause { Conditions = [] };

        // Act
        var kind = sut.Kind;

        // Assert
        kind.Should().Be(expected);
    }

    [Fact]
    public void Conditions_ShouldReturnProvidedConditions_WhenSet()
    {
        // Arrange
        var expected = new[] { new Condition { Column = "name", Value = "kourosh" } };
        var sut = new WhereClause { Conditions = expected };

        // Act
        var conditions = sut.Conditions;

        // Assert
        conditions.Should().Equal(expected);
    }
}