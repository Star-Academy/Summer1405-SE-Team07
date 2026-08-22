using FluentAssertions;
using QueryLib;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;

namespace QueryBuilder.Test;

public class QueryTests
{
    private readonly Query _sut;

    public QueryTests()
    {
        _sut =  new Query();
    }

    [Fact]
    public void Table_ShouldReturnSetTableName_WhenFromWasCalled()
    {
        // Arrange
        _sut.From("student");

        // Act
        var table = _sut.Table;

        // Assert
        table.Should().Be("student");
    }

    [Fact]
    public void Columns_ShouldContainAddedColumns_WhenSelectIsCalledWithColumns()
    {
        // Arrange
        _sut.Select("id", "name");

        // Act
        var columns = _sut.Columns;

        // Assert
        columns.Should().Equal("id", "name");
    }

    [Fact]
    public void Columns_ShouldRemainEmpty_WhenSelectIsCalledWithNull()
    {
        // Arrange
        _sut.Select(null);

        // Act
        var columns = _sut.Columns;

        // Assert
        columns.Should().BeEmpty();
    }

    [Fact]
    public void Clauses_ShouldContainSelectAndFromClause_WhenQueryIsConstructed()
    {
        // Arrange
        var expected = new[] { typeof(SelectClause), typeof(FromClause) };

        // Act
        var clauseTypes = _sut.Clauses.Select(clause => clause.GetType());

        // Assert
        clauseTypes.Should().Equal(expected);
    }

    [Fact]
    public void Where_ShouldAddSingleWhereClauseWithAllConditions_WhenCalledMultipleTimes()
    {
        // Arrange
        _sut.Where("name", "kourosh").Where("age", 20);

        // Act
        var whereClause = (WhereClause)_sut.Clauses.Single(clause => clause is WhereClause);

        // Assert
        whereClause.Conditions.Should()
            .BeEquivalentTo(new[]
            {
                new Condition { Column = "name", Value = "kourosh" },
                new Condition { Column = "age", Value = 20 }
            });
    }

    [Fact]
    public void AddClause_ShouldThrowArgumentNullException_WhenClauseIsNull()
    {
        // Arrange
        IQueryClause? clause = null;

        // Act
        var act = () => _sut.AddClause(clause!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("clause");
    }

    [Fact]
    public void AddClause_ShouldAppendClauseToClauses_WhenClauseIsProvided()
    {
        // Arrange
        var customClause = new FromClause();

        // Act
        _sut.AddClause(customClause);

        // Assert
        _sut.Clauses.Should().Contain(customClause);
    }

    [Fact]
    public void From_Select_Where_ShouldReturnSameInstance_ForFluentChaining()
    {
        // Arrange
        var expected = _sut;

        // Act
        var result = _sut.From("student").Select("id").Where("id", 1);

        // Assert
        result.Should().BeSameAs(expected);
    }
}