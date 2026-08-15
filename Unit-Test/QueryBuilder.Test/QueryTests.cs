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
    public void Constructor_ShouldAddSelectAndFromClauses()
    {
        _sut.Clauses.Should().HaveCount(2);
        _sut.Clauses.ElementAt(0).Should().BeOfType<SelectClause>();
        _sut.Clauses.ElementAt(1).Should().BeOfType<FromClause>();
        _sut.Columns.Should().BeEmpty();
    }

    

  

    [Fact]
    public void From_ShouldReplacePreviouslySelectedTable()
    {
        _sut.From("student");

        _sut.From("teacher");

        _sut.Table.Should().Be("teacher");
    }

    [Fact]
    public void Select_ShouldAddColumnsAndReturnSameQuery()
    {
        var result = _sut.Select("id", "name");

        result.Should().BeSameAs(_sut);
        _sut.Columns.Should().Equal("id", "name");
    }

    [Fact]
    public void Select_ShouldAppendColumns_WhenCalledMoreThanOnce()
    {
        _sut.Select("id");

        _sut.Select("name", "age");

        _sut.Columns.Should().Equal("id", "name", "age");
    }

    [Fact]
    public void Select_ShouldIgnoreNullColumnsAndReturnSameQuery()
    {
        var result = _sut.Select(null);

        result.Should().BeSameAs(_sut);
        _sut.Columns.Should().BeEmpty();
    }

    [Fact]
    public void Where_ShouldAddOneWhereClauseAndReturnSameQuery()
    {
        var result = _sut.Where("id", 10);

        result.Should().BeSameAs(_sut);
        _sut.Clauses.Should().HaveCount(3);
        _sut.Clauses.OfType<WhereClause>().Should().ContainSingle();
    }

    [Fact]
    public void Where_ShouldReuseWhereClause_WhenCalledMoreThanOnce()
    {
        _sut.Where("id", 10);

        _sut.Where("name", "kourosh");

        _sut.Clauses.Should().HaveCount(3);
        _sut.Clauses.OfType<WhereClause>().Should().ContainSingle();
    }

    [Fact]
    public void AddClause_ShouldAddProvidedClauseAndReturnSameQuery()
    {
        var clause = Substitute.For<IQueryClause>();

        var result = _sut.AddClause(clause);

        result.Should().BeSameAs(_sut);
        _sut.Clauses.Should().Contain(clause);
    }

    [Fact]
    public void AddClause_ShouldThrowException_WhenClauseIsNull()
    {
        var act = () => _sut.AddClause(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("clause");
    }
}
