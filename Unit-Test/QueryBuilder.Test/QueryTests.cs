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
        var actual = _sut.Clauses.Select(clause => clause.GetType());

        // Assert
        actual.Should().Equal(expected);
    }

    [Fact]
    public void From_ShouldAddFromClauseWithSpecifiedTable_Whenever()
    {
        // Arrange
        const string expectedTable = "student";
        _sut.From(expectedTable);

        // Act
        var actual = _sut.Clauses.OfType<FromClause>().Single();

        // Assert
        actual.Table.Should().Be(expectedTable);
    }

    [Fact]
    public void Select_ShouldAddColumnsToSelectClause_WhenCalledWithColumns()
    {
        // Arrange
        var expected = new[] { "id", "name" };
        _sut.Select("id", "name");

        // Act
        var actual = _sut.Clauses.OfType<SelectClause>().Single();

        // Assert
        actual.Columns.Should().Equal(expected);
    }

    [Fact]
    public void Select_ShouldNotAddColumns_WhenCalledWithNull()
    {
        // Arrange
        _sut.Select(null);

        // Act
        var actual = _sut.Clauses.OfType<SelectClause>().Single();

        // Assert
        actual.Columns.Should().BeEmpty();
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
        var actual = _sut.Clauses.OfType<WhereClause>().Single();

        // Assert
        actual.Conditions.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void From_ShouldReturnSameQueryInstance_WhenCalledInFluentChaining()
    {
        // Arrange
        var expected = _sut;

        // Act
        var actual = _sut.From("student").Select("id").Where("id", 1);

        // Assert
        actual.Should().BeSameAs(expected);
    }
}