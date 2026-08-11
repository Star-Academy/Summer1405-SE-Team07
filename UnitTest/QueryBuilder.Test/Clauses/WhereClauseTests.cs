using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Clauses;
using QueryLib.Dialects.Abstractions;

namespace QueryBuilder.Test.Clauses;

public sealed class WhereClauseTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();
    private readonly IParameterPlaceholderFactory _placeholders =
        Substitute.For<IParameterPlaceholderFactory>();
    private readonly WhereClause _sut = new();

    [Fact]
    public void Render_ShouldGenerateWhereClause_WhenConditionIsAdded()
    {
        _sut.Add(new Condition { Column = "name", Value = "kourosh" });
        _quoter.Quote("name").Returns("\"name\"");
        _placeholders.MakePlaceholder(1).Returns("$1");

        var output = _sut.Render(
            _quoter,
            _placeholders,
            Array.Empty<object?>());

        output.Sql.Should().Be("WHERE \"name\" = $1");
        output.Bindings.Should().Equal("kourosh");
    }

    [Fact]
    public void Render_ShouldReturnEmptySql_WhenNoConditionsExist()
    {
        var existingBindings = new object?[] { 10 };

        var output = _sut.Render(_quoter, _placeholders, existingBindings);

        output.Sql.Should().BeEmpty();
        output.Bindings.Should().Equal(10);
    }

    [Fact]
    public void Render_ShouldJoinConditionsWithAndAndPreserveBindingOrder()
    {
        _sut.Add(new Condition { Column = "name", Value = "kourosh" });
        _sut.Add(new Condition { Column = "age", Value = 20 });
        _quoter.Quote("name").Returns("\"name\"");
        _quoter.Quote("age").Returns("\"age\"");
        _placeholders.MakePlaceholder(1).Returns("$1");
        _placeholders.MakePlaceholder(2).Returns("$2");

        var output = _sut.Render(
            _quoter,
            _placeholders,
            Array.Empty<object?>());

        output.Sql.Should().Be("WHERE \"name\" = $1 AND \"age\" = $2");
        output.Bindings.Should().Equal("kourosh", 20);
    }

    [Fact]
    public void Render_ShouldPreserveExistingBindingsAndContinuePlaceholderNumbering()
    {
        var existingBindings = new object?[] { 10 };
        _sut.Add(new Condition { Column = "name", Value = "kourosh" });
        _quoter.Quote("name").Returns("\"name\"");
        _placeholders.MakePlaceholder(2).Returns("$2");

        var output = _sut.Render(_quoter, _placeholders, existingBindings);

        output.Sql.Should().Be("WHERE \"name\" = $2");
        output.Bindings.Should().Equal(10, "kourosh");
        _placeholders.Received(1).MakePlaceholder(2);
    }

    [Fact]
    public void Render_ShouldNotModifyInputBindings()
    {
        var existingBindings = new List<object?> { 10 };
        _sut.Add(new Condition { Column = "name", Value = "kourosh" });
        _quoter.Quote("name").Returns("\"name\"");
        _placeholders.MakePlaceholder(2).Returns("$2");

        var output = _sut.Render(_quoter, _placeholders, existingBindings);

        existingBindings.Should().Equal(10);
        output.Bindings.Should().Equal(10, "kourosh");
        output.Bindings.Should().NotBeSameAs(existingBindings);
    }

    [Fact]
    public void Add_ShouldSetHasConditionsToTrue()
    {
        _sut.HasConditions.Should().BeFalse();

        _sut.Add(new Condition { Column = "name", Value = "kourosh" });

        _sut.HasConditions.Should().BeTrue();
    }
}
