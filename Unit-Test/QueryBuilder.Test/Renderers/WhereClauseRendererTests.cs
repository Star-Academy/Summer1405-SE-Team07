using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers;

namespace QueryBuilder.Test.Renderers;

public class WhereClauseRendererTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();
    private readonly IParameterPlaceholderFactory _placeholders = Substitute.For<IParameterPlaceholderFactory>();

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQuoterIsNull()
    {
        // Arrange
        var act = () => new WhereClauseRenderer(DbProvider.PostgreSql, null!, _placeholders);

        // Act
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("quoter");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenPlaceholdersIsNull()
    {
        // Arrange
        var act = () => new WhereClauseRenderer(DbProvider.PostgreSql, _quoter, null!);

        // Act
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("placeholders");
    }

    [Fact]
    public void ClauseKind_ShouldBeWhere_Whenever()
    {
        // Arrange
        var sut = new WhereClauseRenderer(DbProvider.PostgreSql, _quoter, _placeholders);

        // Act
        var kind = sut.ClauseKind;

        // Assert
        kind.Should().Be(ClauseKind.Where);
    }

    [Theory]
    [InlineData(DbProvider.PostgreSql)]
    [InlineData(DbProvider.SqlServer)]
    public void Provider_ShouldReturnConfiguredProvider_WhenConstructed(DbProvider provider)
    {
        // Arrange
        var sut = new WhereClauseRenderer(provider, _quoter, _placeholders);

        // Act
        var result = sut.Provider;

        // Assert
        result.Should().Be(provider);
    }

    [Theory]
    [InlineData(DbProvider.PostgreSql)]
    [InlineData(DbProvider.SqlServer)]
    public void Render_ShouldReturnEmptySql_WhenNoConditionsExist(DbProvider provider)
    {
        // Arrange
        var sut = new WhereClauseRenderer(provider, _quoter, _placeholders);
        var clause = new WhereClause { Conditions = [] };
        var expected = new RenderOutput(string.Empty, Array.Empty<object?>());

        // Act
        var output = sut.Render(clause);

        // Assert
        output.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(DbProvider.PostgreSql)]
    [InlineData(DbProvider.SqlServer)]
    public void Render_ShouldGenerateWhereClauseAndBinding_WhenOneConditionExists(DbProvider provider)
    {
        // Arrange
        var sut = new WhereClauseRenderer(provider, _quoter, _placeholders);
        var clause = new WhereClause
        {
            Conditions = [new Condition { Column = "name", Value = "kourosh" }]
        };
        _quoter.Quote("name").Returns("\"name\"");
        _placeholders.MakePlaceholder(1).Returns("$1");
        var expected = new RenderOutput("WHERE \"name\" = $1", new object?[] { "kourosh" });

        // Act
        var output = sut.Render(clause);

        // Assert
        output.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(DbProvider.PostgreSql)]
    [InlineData(DbProvider.SqlServer)]
    public void Render_ShouldJoinWithAnd_WhenMultipleConditionsExist(DbProvider provider)
    {
        // Arrange
        var sut = new WhereClauseRenderer(provider, _quoter, _placeholders);
        var clause = new WhereClause
        {
            Conditions =
            [
                new Condition { Column = "name", Value = "kourosh" },
                new Condition { Column = "age", Value = 20 }
            ]
        };
        _quoter.Quote("name").Returns("\"name\"");
        _quoter.Quote("age").Returns("\"age\"");
        _placeholders.MakePlaceholder(1).Returns("$1");
        _placeholders.MakePlaceholder(2).Returns("$2");
        var expected = new RenderOutput("WHERE \"name\" = $1 AND \"age\" = $2", new object?[] { "kourosh", 20 });

        // Act
        var output = sut.Render(clause);

        // Assert
        output.Should().BeEquivalentTo(expected);
    }
}