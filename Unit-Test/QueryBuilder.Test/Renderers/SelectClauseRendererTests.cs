using FluentAssertions;
using NSubstitute;
using QueryLib.Clauses;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers;

namespace QueryBuilder.Test.Renderers;

public class SelectClauseRendererTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();
    private readonly SelectClauseRenderer _sut;

    public SelectClauseRendererTests()
    {
        _sut = new SelectClauseRenderer(_quoter);
    }

    [Fact]
    public void ClauseType_ShouldBeSelectClause()
    {
        // Arrange
        
        // Act
        
        // Assert
        _sut.ClauseType.Should().Be(typeof(SelectClause));
    }

    [Fact]
    public void Render_ShouldSelectAllColumns_WhenNoColumnsProvided()
    {
        // Arrange
        var clause = new SelectClause { Columns = [] };

        // Act
        var output = _sut.Render(clause, Array.Empty<object?>());

        // Assert
        output.Sql.Should().Be("SELECT *");
    }

    [Fact]
    public void Render_ShouldQuoteEveryColumn_AndPreserveBindings_WhenColumnsProvided()
    {
        // Arrange
        var clause = new SelectClause { Columns = ["id", "name"] };
        _quoter.Quote("id").Returns("\"id\"");
        _quoter.Quote("name").Returns("\"name\"");
        var bindings = new object?[] { 5 };

        // Act
        var output = _sut.Render(clause, bindings);

        // Assert
        output.Sql.Should().Be("SELECT \"id\", \"name\"");
        output.Bindings.Should().BeSameAs(bindings);
    }
}