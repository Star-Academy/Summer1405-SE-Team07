using FluentAssertions;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;

namespace QueryBuilder.Test.Clauses;

public class FromClauseTests
{
    private readonly FromClause _sut;
    public FromClauseTests()
    {
        _sut =  new FromClause();
    }
        
    [Fact]
    public void SetTable_ShouldThrowArgumentException_WhenTableNameIsWhitespace()
    {
        // Arrange
        const string table = " ";
        const string expected = "table";

        // Act
        var act = () => _sut.SetTable(table);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName(expected);
    }

    [Fact]
    public void Table_ShouldThrowInvalidOperationException_WhenTableIsNotSet()
    {
        // Arrange
        const string expected = "From(...) must be called before compiling the query.";

        // Act
        var act = () => _sut.Table;

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage(expected);
    }

    [Fact]
    public void Table_ShouldReturnSetValue_WhenSetTableWasCalled()
    {
        // Arrange
        _sut.SetTable("student");

        // Act
        var table = _sut.Table;

        // Assert
        table.Should().Be("student");
    }

    [Fact]
    public void Order_ShouldBe10()
    {
        // Arrange

        // Act
        
        // Assert
        _sut.Order.Should().Be(10);
    }
}