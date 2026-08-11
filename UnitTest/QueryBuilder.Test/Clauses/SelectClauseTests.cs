using FluentAssertions;
using NSubstitute;
using QueryLib.Clauses;
using QueryLib.Dialects.Abstractions;

namespace QueryBuilder.Test.Clauses;

public sealed class SelectClauseTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();
    private readonly IParameterPlaceholderFactory _placeholders =
        Substitute.For<IParameterPlaceholderFactory>();

    [Fact]
    public void Render_ShouldSelectAllColumns_WhenNoColumnsWereAdded()
    {
        var clause = new SelectClause();

        var output = clause.Render(_quoter, _placeholders, Array.Empty<object?>());

        output.Sql.Should().Be("SELECT *");
    }

    [Fact]
    public void Render_ShouldQuoteEverySelectedColumn()
    {
        var clause = new SelectClause();
        clause.Add(["id", "name"]);
        _quoter.Quote("id").Returns("\"id\"");
        _quoter.Quote("name").Returns("\"name\"");

        var output = clause.Render(_quoter, _placeholders, Array.Empty<object?>());

        output.Sql.Should().Be("SELECT \"id\", \"name\"");
    }
}
