using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;

namespace QueryBuilder.Test.Clauses;

public sealed class WhereClauseTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();
    private readonly IParameterPlaceholderFactory _placeholders =
        Substitute.For<IParameterPlaceholderFactory>();
    private readonly WhereClause _sut = new();

    [Fact]
    public void Render_ShouldGenerateWhereClauseAndBinding_WhenOneConditionExists()
    {
        // Arrange
        _sut.Add(new Condition { Column = "name", Value = "kourosh" });
        _quoter.Quote("name").Returns("\"name\"");
        _placeholders.MakePlaceholder(1).Returns("$1");
        var expected = new RenderOutput("WHERE \"name\" = $1" , new object?[] { "kourosh" });

        // Act
        var output = _sut.Render(
            _quoter,
            _placeholders,
            Array.Empty<object?>());
        
        // Assert
        output.Should().BeEquivalentTo(expected);
        
    }

    [Fact]
    public void Render_ShouldReturnEmptySqlAndPreserveBindings_WhenNoConditionsExist()
    {
        // Arrange
        var existingBindings = new object?[] { 10 };
        var expected = new RenderOutput("" , existingBindings);

        // Act
        var output = _sut.Render(_quoter, _placeholders, existingBindings);

        // Assert
        output.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Render_ShouldJoinWithAndAndPreserveBindingOrder_WhenMultipleConditionsExist()
    {
        // Arrange
        _sut.Add(new Condition { Column = "name", Value = "kourosh" });
        _sut.Add(new Condition { Column = "age", Value = 20 });
        _quoter.Quote("name").Returns("\"name\"");
        _quoter.Quote("age").Returns("\"age\"");
        _placeholders.MakePlaceholder(1).Returns("$1");
        _placeholders.MakePlaceholder(2).Returns("$2");
        var expected = new RenderOutput("WHERE \"name\" = $1 AND \"age\" = $2" ,
            new object?[] { "kourosh", 20 });
        
        // Act
        var output = _sut.Render(
            _quoter,
            _placeholders,
            Array.Empty<object?>());

        // Assert
        output.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Render_ShouldPreserveThemAndContinuePlaceholderNumbering_WhenBindingsAlreadyExist()
    {
        // Arrange
        var existingBindings = new object?[] { 10 };
        _sut.Add(new Condition { Column = "name", Value = "kourosh" });
        _quoter.Quote("name").Returns("\"name\"");
        _placeholders.MakePlaceholder(2).Returns("$2");
        var expected = new RenderOutput("WHERE \"name\" = $2" ,
            new object?[] { 10 , "kourosh" });

        // Act
        var output = _sut.Render(_quoter, _placeholders, existingBindings);

        // Assert
        output.Should().BeEquivalentTo(expected);
        _placeholders.Received(1).MakePlaceholder(2);
    }

    [Fact]
    public void Render_ShouldReturnNewBindingsWithoutModifyingInput_WhenBindingsAreProvided()
    {
        // Arrange
        var existingBindings = new List<object?> { 10 };
        _sut.Add(new Condition { Column = "name", Value = "kourosh" });
        _quoter.Quote("name").Returns("\"name\"");
        _placeholders.MakePlaceholder(2).Returns("$2");
        var expectedBindings = new object?[] { 10, "kourosh" };

        // Act
        var output = _sut.Render(_quoter, _placeholders, existingBindings);

        // Assert
        existingBindings.Should().Equal(10);
        output.Bindings.Should()
            .Equal(expectedBindings)
            .And.NotBeSameAs(existingBindings);
    }

    [Fact]
    public void Add_ShouldSetHasConditionsToTrue_WhenConditionIsProvided()
    {
        // Arrange
        var condition = new Condition { Column = "name", Value = "kourosh" };

        // Act
        _sut.Add(condition);

        // Assert
        _sut.HasConditions.Should().BeTrue();
    }
}
