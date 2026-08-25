using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers;

namespace QueryBuilder.Test.Renderers;

public class SelectClauseRendererTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQuoterIsNull()
    {
        // Arrange
        var act = () => new SelectClauseRenderer(DbProvider.PostgreSql, null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("quoter");
    }

    [Fact]
    public void ClauseKind_ShouldBeSelect()
    {
        // Arrange
        var sut = new SelectClauseRenderer(DbProvider.PostgreSql, _quoter);

        // Act
        var kind = sut.ClauseKind;

        // Assert
        kind.Should().Be(ClauseKind.Select);
    }

    [Theory]
    [InlineData(DbProvider.PostgreSql)]
    [InlineData(DbProvider.SqlServer)]
    public void Provider_ShouldReturnConfiguredProvider(DbProvider provider)
    {
        // Arrange
        var sut = new SelectClauseRenderer(provider, _quoter);

        // Act
        var result = sut.Provider;

        // Assert
        result.Should().Be(provider);
    }

    [Theory]
    [InlineData(DbProvider.PostgreSql)]
    [InlineData(DbProvider.SqlServer)]
    public void Render_ShouldSelectAllColumns_WhenNoColumnsProvided(DbProvider provider)
    {
        // Arrange
        var sut = new SelectClauseRenderer(provider, _quoter);
        var clause = new SelectClause { Columns = [] };
        var expected = new RenderOutput("SELECT *", Array.Empty<object?>());

        // Act
        var output = sut.Render(clause);

        // Assert
        output.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(DbProvider.PostgreSql)]
    [InlineData(DbProvider.SqlServer)]
    public void Render_ShouldQuoteEveryColumn_WhenColumnsProvided(DbProvider provider)
    {
        // Arrange
        var sut = new SelectClauseRenderer(provider, _quoter);
        var clause = new SelectClause { Columns = ["id", "name"] };
        _quoter.Quote("id").Returns("\"id\"");
        _quoter.Quote("name").Returns("\"name\"");
        var expected = new RenderOutput("SELECT \"id\", \"name\"", Array.Empty<object?>());

        // Act
        var output = sut.Render(clause);

        // Assert
        output.Should().BeEquivalentTo(expected);
    }
}