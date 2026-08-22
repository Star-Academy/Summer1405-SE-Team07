using FluentAssertions;
using NSubstitute;
using QueryLib.Clauses;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers;

namespace QueryBuilder.Test.Renderers;

public class FromClauseRendererTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();
    private readonly FromClauseRenderer _sut;

    public FromClauseRendererTests()
    {
        _sut = new FromClauseRenderer(_quoter);
    }

    [Fact]
    public void ClauseType_ShouldBeFromClause()
    {
        // Arrange
        
        // Act
        
        // Assert
        _sut.ClauseType.Should().Be(typeof(FromClause));
    }

    [Fact]
    public void Render_ShouldReturnQuotedTableName_AndPreserveBindings()
    {
        // Arrange
        var clause = new FromClause();
        clause.SetTable("student");
        _quoter.Quote("student").Returns("\"student\"");
        var bindings = new object?[] { 1 };

        // Act
        var output = _sut.Render(clause, bindings);

        // Assert
        output.Sql.Should().Be("FROM \"student\"");
        output.Bindings.Should().BeSameAs(bindings);
    }
}