using FluentAssertions;
using NSubstitute;
using QueryLib.Clauses;
using QueryLib.Dialects.Abstractions;

namespace QueryBuilder.Test.Clauses;

public sealed class FromClauseTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();
    private readonly IParameterPlaceholderFactory _placeholders =
        Substitute.For<IParameterPlaceholderFactory>();

    [Fact]
    public void SetTable_ShouldRejectAnEmptyTableName()
    {
        var clause = new FromClause();

        var act = () => clause.SetTable(" ");

        act.Should().Throw<ArgumentException>().WithParameterName("table");
    }

    [Fact]
    public void Render_ShouldRequireATable()
    {
        var clause = new FromClause();

        var act = () => clause.Render(
            _quoter,
            _placeholders,
            Array.Empty<object?>());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("From(...) must be called before compiling the query.");
    }

    [Fact]
    public void Render_ShouldQuoteTheTableName()
    {
        var clause = new FromClause();
        clause.SetTable("student");
        _quoter.Quote("student").Returns("\"student\"");

        var output = clause.Render(_quoter, _placeholders, Array.Empty<object?>());

        output.Sql.Should().Be("FROM \"student\"");
    }
}
