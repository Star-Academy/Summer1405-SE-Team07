using FluentAssertions;
using QueryLib;
using QueryLib.Clauses;

namespace QueryBuilder.Test;

public class QueryTests
{
    private readonly Query _sut;

    public QueryTests()
    {
        _sut = new Query();
    }

    [Fact]
    public void Clauses_ShouldContainOnlySelectClause_WhenQueryIsConstructed()
    {
        // Arrange
        var expected = new[] { typeof(SelectClause) };

        // Act
        var clauseTypes = _sut.Clauses.Select(clause => clause.GetType());

        // Assert
        clauseTypes.Should().Equal(expected);
    }

    [Fact]
    public void From_ShouldAddFromClauseWithSpecifiedTable_WhenCalled()
    {
        // Arrange
        const string expectedTable = "student";
        _sut.From(expectedTable);

        // Act
        var fromClause = _sut.Clauses.OfType<FromClause>().Single();

        // Assert
        fromClause.Table.Should().Be(expectedTable);
    }

    [Fact]
    public void Select_ShouldAddColumnsToSelectClause_WhenCalledWithColumns()
    {
        // Arrange
        var expected = new[] { "id", "name" };
        _sut.Select("id", "name");

        // Act
        var selectClause = _sut.Clauses.OfType<SelectClause>().Single();

        // Assert
        selectClause.Columns.Should().Equal(expected);
    }

    [Fact]
    public void Select_ShouldNotAddColumns_WhenCalledWithNull()
    {
        // Arrange
        _sut.Select(null);

        // Act
        var selectClause = _sut.Clauses.OfType<SelectClause>().Single();

        // Assert
        selectClause.Columns.Should().BeEmpty();
    }

    [Fact]
    public void Where_ShouldAddSingleWhereClauseWithAllConditions_WhenCalledMultipleTimes()
    {
        // Arrange
        var expected = new[]
        {
            new Condition { Column = "name", Value = "kourosh" },
            new Condition { Column = "age", Value = 20 }
        };
        _sut.Where("name", "kourosh").Where("age", 20);

        // Act
        var whereClause = _sut.Clauses.OfType<WhereClause>().Single();

        // Assert
        whereClause.Conditions.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void From_ShouldReturnSameQueryInstance_WhenCalledInFluentChaining()
    {
        // Arrange
        var expected = _sut;

        // Act
        var result = _sut.From("student").Select("id").Where("id", 1);

        // Assert
        result.Should().BeSameAs(expected);
    }
}