using FluentAssertions;
using NSubstitute;
using QueryLib.Clauses;
using QueryLib.Dialects.Abstractions;

namespace QueryBuilder.Test.Clauses;

public class SelectClauseTests
{
    private readonly IIdentifierQuoter _quoter;
    private readonly IParameterPlaceholderFactory _placeholders;
    private readonly SelectClause _sut;

    public SelectClauseTests()
    {
        _quoter = Substitute.For<IIdentifierQuoter>();
        _placeholders = Substitute.For<IParameterPlaceholderFactory>();
        _sut = new SelectClause();
    }
    [Fact]
    public void Render_ShouldSelectAllColumns_WhenNoColumnsWereAdded()
    {
        // Arrange
        var clause = new SelectClause();

        // Act
        var output = clause.Render(_quoter, _placeholders, Array.Empty<object?>());

        // Assert
        output.Sql.Should().Be("SELECT *");
    }

    [Fact]
    public void Render_ShouldQuoteEverySelectedColumn_WhenColumnsWereAdded()
    {
        // Arrange
        _sut.Add(["id", "name"]);
        _quoter.Quote("id").Returns("\"id\"");
        _quoter.Quote("name").Returns("\"name\"");

        // Act
        var output = _sut.Render(_quoter, _placeholders, Array.Empty<object?>());

        // Assert
        output.Sql.Should().Be("SELECT \"id\", \"name\"");
    }

    [Fact]
    public void Add_ShouldLeaveColumnsEmpty_WhenColumnsAreNull()
    {
        // Arrange
        string[]? columns = null;

        // Act
        _sut.Add(columns);

        // Assert
        _sut.Columns.Should().BeEmpty();
    }
}
