using FluentAssertions;
using NSubstitute;
using QueryLib.Clauses;
using QueryLib.Dialects.Abstractions;

namespace QueryBuilder.Test.Clauses;

public class SelectClauseTests
{
    private readonly IIdentifierQuoter _quoter;
    private readonly IParameterPlaceholderFactory _placeholders;
    private readonly SelectClause _sut;

    public SelectClauseTests()
    {
        _quoter = Substitute.For<IIdentifierQuoter>();
        _placeholders = Substitute.For<IParameterPlaceholderFactory>();
        _sut = new SelectClause();
    }


    [Fact]
    public void Render_ShouldSelectAllColumns_WhenNoColumnsWereAdded()
    {

        var output = _sut.Render(_quoter, _placeholders, Array.Empty<object?>());

        output.Sql.Should().Be("SELECT *");
    }

    [Fact]
    public void Render_ShouldQuoteEverySelectedColumn()
    {
        _sut.Add(["id", "name"]);
        _quoter.Quote("id").Returns("\"id\"");
        _quoter.Quote("name").Returns("\"name\"");

        var output = _sut.Render(_quoter, _placeholders, Array.Empty<object?>());

        output.Sql.Should().Be("SELECT \"id\", \"name\"");
    }

    [Fact]
    public void Add_ShouldIgnoreNullColumns()
    {
        _sut.Add(null);

        _sut.Columns.Should().BeEmpty();
    }
}
