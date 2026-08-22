using FluentAssertions;
using NSubstitute;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers;

namespace QueryBuilder.Test.Renderers;

public class WhereClauseRendererTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();
    private readonly IParameterPlaceholderFactory _placeholders = Substitute.For<IParameterPlaceholderFactory>();
    private readonly WhereClauseRenderer _sut;

    public WhereClauseRendererTests()
    {
        _sut = new WhereClauseRenderer(_quoter, _placeholders);
    }

    [Fact]
    public void ClauseType_ShouldBeWhereClause()
    {
        // Arrange
        
        // Act
        
        // Assert
        _sut.ClauseType.Should().Be(typeof(WhereClause));
    }

    [Fact]
    public void Render_ShouldReturnEmptySql_AndPreserveExistingBindings_WhenNoConditionsExist()
    {
        // Arrange
        var clause = new WhereClause { Conditions = [] };
        var existingBindings = new object?[] { 10 };

        // Act
        var output = _sut.Render(clause, existingBindings);

        // Assert
        output.Sql.Should().BeEmpty();
        output.Bindings.Should().Equal(existingBindings);
    }

    [Fact]
    public void Render_ShouldGenerateWhereClauseAndBinding_WhenOneConditionExists()
    {
        // Arrange
        var clause = new WhereClause
        {
            Conditions = [new Condition { Column = "name", Value = "kourosh" }]
        };
        _quoter.Quote("name").Returns("\"name\"");
        _placeholders.MakePlaceholder(1).Returns("$1");

        // Act
        var output = _sut.Render(clause, Array.Empty<object?>());

        // Assert
        output.Sql.Should().Be("WHERE \"name\" = $1");
        output.Bindings.Should().Equal("kourosh");
    }

    [Fact]
    public void Render_ShouldJoinWithAnd_AndPreserveBindingOrder_WhenMultipleConditionsExist()
    {
        // Arrange
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

        // Act
        var output = _sut.Render(clause, Array.Empty<object?>());

        // Assert
        output.Sql.Should().Be("WHERE \"name\" = $1 AND \"age\" = $2");
        output.Bindings.Should().Equal("kourosh", 20);
    }

    [Fact]
    public void Render_ShouldContinuePlaceholderNumbering_AndNotModifyInput_WhenBindingsAlreadyExist()
    {
        // Arrange
        var existingBindings = new List<object?> { 10 };
        var clause = new WhereClause
        {
            Conditions = [new Condition { Column = "name", Value = "kourosh" }]
        };
        _quoter.Quote("name").Returns("\"name\"");
        _placeholders.MakePlaceholder(2).Returns("$2");

        // Act
        var output = _sut.Render(clause, existingBindings);

        // Assert
        existingBindings.Should().Equal(10);
        output.Sql.Should().Be("WHERE \"name\" = $2");
        output.Bindings.Should().Equal(10, "kourosh");
        _placeholders.Received(1).MakePlaceholder(2);
    }
}