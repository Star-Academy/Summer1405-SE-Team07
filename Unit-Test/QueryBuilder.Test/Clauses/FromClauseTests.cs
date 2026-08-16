using FluentAssertions;
using NSubstitute;
using QueryLib.Clauses;
using QueryLib.Dialects.Abstractions;

namespace QueryBuilder.Test.Clauses;

public class FromClauseTests
{
    private readonly IIdentifierQuoter _quoter;
    private readonly IParameterPlaceholderFactory _placeholders;
    private readonly FromClause _sut;

    public FromClauseTests()
    {
        _quoter = Substitute.For<IIdentifierQuoter>();
        _placeholders = Substitute.For<IParameterPlaceholderFactory>();
        _sut = new FromClause();
    }

    [Fact]
    public void SetTable_WhenTableNameIsWhitespace_ShouldThrowArgumentException()
    {
        // Arrange
        const string table = " ";

        // Act
        var act = () => _sut.SetTable(table);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("table");
    }

    [Fact]
    public void Render_WhenTableIsNotSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var clause = new FromClause();

        // Act
        var act = () => clause.Render(
            _quoter,
            _placeholders,
            Array.Empty<object?>());

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("From(...) must be called before compiling the query.");
    }

    [Fact]
    public void Render_WhenTableIsSet_ShouldReturnQuotedTableName()
    {
        // Arrange
        _sut.SetTable("student");
        _quoter.Quote("student").Returns("\"student\"");

        // Act
        var output = _sut.Render(_quoter, _placeholders, Array.Empty<object?>());

        // Assert
        output.Sql.Should().Be("FROM \"student\"");
    }
}
