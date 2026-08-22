using FluentAssertions;
using QueryLib.Clauses;

namespace QueryBuilder.Test.Clauses;

public class WhereClauseTests
{
    [Fact]
    public void HasConditions_ShouldBeFalse_WhenConditionsIsEmpty()
    {
        // Arrange
        var clause = new WhereClause { Conditions = [] };

        // Act
        
        // Assert
        clause.HasConditions.Should().BeFalse();
    }

    [Fact]
    public void HasConditions_ShouldBeTrue_WhenConditionsIsNotEmpty()
    {
        // Arrange
        var clause = new WhereClause
        {
            Conditions = [new Condition { Column = "name", Value = "kourosh" }]
        };

        // Act
        
        // Assert
        clause.HasConditions.Should().BeTrue();
    }

    [Fact]
    public void Order_ShouldBe20()
    {
        // Arrange
        var clause = new WhereClause { Conditions = [] };

        // Act
        
        // Assert
        clause.Order.Should().Be(20);
    }
}