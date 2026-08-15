using FluentAssertions;
using NSubstitute;
using QueryLib.Clauses;
using QueryLib.Dialects.Abstractions;

namespace QueryBuilder.Test.Clauses;

public  class FromClauseTests
{
    private readonly IIdentifierQuoter _quoter;
    private readonly IParameterPlaceholderFactory _placeholders ;
    private readonly FromClause _sut;

    public FromClauseTests()
    {
        _quoter = Substitute.For<IIdentifierQuoter>();
        _placeholders = Substitute.For<IParameterPlaceholderFactory>();
        _sut = new FromClause();
    }

    [Fact]
    public void SetTable_ShouldRejectAnEmptyTableName()
    {
        var act = () => _sut.SetTable(" ");
        act.Should().Throw<ArgumentException>().WithParameterName("table");
    }

    [Fact]
    public void Render_ShouldRequireATable()
    {


        var act = () => _sut.Render(
            _quoter,
            _placeholders,
            Array.Empty<object?>());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("From(...) must be called before compiling the query.");
    }

    [Fact]
    public void Render_ShouldQuoteTheTableName()
    {

        _sut.SetTable("student");
        _quoter.Quote("student").Returns("\"student\"");

        var output = _sut.Render(_quoter, _placeholders, Array.Empty<object?>());

        output.Sql.Should().Be("FROM \"student\"");
    }
}
