using FluentAssertions;
using QueryLib.Clauses;

namespace QueryBuilder.Test.Clauses;

public class SelectClauseTests
{
    [Fact]
    public void Columns_ShouldReturnProvidedColumns_WhenSet()
    {
        // Arrange
        var clause = new SelectClause { Columns = ["id", "name"] };

        // Act
        
        // Assert
        clause.Columns.Should().Equal("id", "name");
    }

    [Fact]
    public void Order_ShouldBe0()
    {
        // Arrange
        var clause = new SelectClause { Columns = [] };

        // Act
        
        // Assert
        clause.Order.Should().Be(0);
    }
}