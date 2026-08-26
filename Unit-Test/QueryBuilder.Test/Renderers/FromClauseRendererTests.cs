using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers;

namespace QueryBuilder.Test.Renderers;

public class FromClauseRendererTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQuoterIsNull()
    {
        // Arrange

        // Act
        var act = () => new FromClauseRenderer(DbProvider.PostgreSql, null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("quoter");
    }

    [Fact]
    public void ClauseKind_ShouldBeFrom_Whenever()
    {
        // Arrange
        var sut = new FromClauseRenderer(DbProvider.PostgreSql, _quoter);

        // Act
        var actual = sut.ClauseKind;

        // Assert
        actual.Should().Be(ClauseKind.From);
    }

    [Theory]
    [InlineData(DbProvider.PostgreSql)]
    [InlineData(DbProvider.SqlServer)]
    public void Provider_ShouldReturnConfiguredProvider_WhenConstructed(DbProvider provider)
    {
        // Arrange
        var sut = new FromClauseRenderer(provider, _quoter);

        // Act
        var actual = sut.Provider;

        // Assert
        actual.Should().Be(provider);
    }

    [Theory]
    [InlineData(DbProvider.PostgreSql)]
    [InlineData(DbProvider.SqlServer)]
    public void Render_ShouldReturnQuotedTableName_Whenever(DbProvider provider)
    {
        // Arrange
        var sut = new FromClauseRenderer(provider, _quoter);
        var clause = new FromClause { Table = "student" };
        _quoter.Quote("student").Returns("\"student\"");
        var expected = new RenderOutput("FROM \"student\"", Array.Empty<object?>());

        // Act
        var actual = sut.Render(clause);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}