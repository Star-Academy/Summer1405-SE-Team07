using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;

namespace QueryBuilder.Test;

public sealed class QueryTests
{
    private readonly Query _sut;

    public QueryTests()
    {
        _sut = new Query();
    }

    [Fact]
    public void Constructor_ShouldInitializeSelectAndFromClauses_WhenCalled()
    {
        // Arrange
        const int expectedClauseCount = 2;

        // Act
        var query = new Query();

        // Assert
        query.Clauses.Should().HaveCount(expectedClauseCount);
        query.Clauses.ElementAt(0).Should().BeOfType<SelectClause>();
        query.Clauses.ElementAt(1).Should().BeOfType<FromClause>();
        query.Columns.Should().BeEmpty();
    }

    [Fact]
    public void From_ShouldReplacePreviouslySelectedTable_WhenCalledMoreThanOnce()
    {
        // Arrange
        _sut.From("student");

        // Act
        _sut.From("teacher");

        // Assert
        _sut.Table.Should().Be("teacher");
    }

    [Fact]
    public void Select_ShouldAddColumnsAndReturnSameQuery_WhenColumnsAreProvided()
    {
        // Arrange
        var columns = new[] { "id", "name" };

        // Act
        var result = _sut.Select(columns);

        // Assert
        result.Should().BeSameAs(_sut);
        _sut.Columns.Should().Equal("id", "name");
    }

    [Fact]
    public void Select_ShouldAppendColumns_WhenCalledMoreThanOnce()
    {
        // Arrange
        _sut.Select("id");

        // Act
        _sut.Select("name", "age");

        // Assert
        _sut.Columns.Should().Equal("id", "name", "age");
    }

    [Fact]
    public void Select_ShouldIgnoreColumnsAndReturnSameQuery_WhenColumnsAreNull()
    {
        // Arrange
        string[]? columns = null;

        // Act
        var result = _sut.Select(columns);

        // Assert
        result.Should().BeSameAs(_sut);
        _sut.Columns.Should().BeEmpty();
    }

    [Fact]
    public void Where_ShouldAddWhereClauseAndReturnSameQuery_WhenCalledForFirstCondition()
    {
        // Arrange
        const string column = "id";
        const int value = 10;

        // Act
        var result = _sut.Where(column, value);

        // Assert
        result.Should().BeSameAs(_sut);
        _sut.Clauses.Should().HaveCount(3);
        _sut.Clauses.OfType<WhereClause>().Should().ContainSingle();
    }

    [Fact]
    public void Where_ShouldReuseExistingWhereClause_WhenCalledMoreThanOnce()
    {
        // Arrange
        _sut.Where("id", 10);

        // Act
        _sut.Where("name", "kourosh");

        // Assert
        _sut.Clauses.Should().HaveCount(3);
        _sut.Clauses.OfType<WhereClause>().Should().ContainSingle();
    }

    [Fact]
    public void AddClause_ShouldAddClauseAndReturnSameQuery_WhenClauseIsProvided()
    {
        // Arrange
        var clause = Substitute.For<IQueryClause>();

        // Act
        var result = _sut.AddClause(clause);

        // Assert
        result.Should().BeSameAs(_sut);
        _sut.Clauses.Should().Contain(clause);
    }

    [Fact]
    public void AddClause_ShouldThrowArgumentNullException_WhenClauseIsNull()
    {
        // Arrange
        IQueryClause? clause = null;

        // Act
        var act = () => _sut.AddClause(clause!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("clause");
    }
}
