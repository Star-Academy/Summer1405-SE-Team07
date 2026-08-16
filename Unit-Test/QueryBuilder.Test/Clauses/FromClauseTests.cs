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
    public void SetTable_ShouldThrowArgumentException_WhenTableNameIsWhitespace()
    {
        // Arrange
        const string table = " ";
        var expected = "table" ;

        // Act
        var act = () => _sut.SetTable(table);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName(expected);
    }

    [Fact]
    public void Render_ShouldThrowInvalidOperationException_WhenTableIsNotSet()
    {
        // Arrange
        var clause = new FromClause();
        var expected = "From(...) must be called before compiling the query." ;

        // Act
        var act = () => clause.Render(
            _quoter,
            _placeholders,
            Array.Empty<object?>());

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage(expected);
    }

    [Fact]
    public void Render_ShouldReturnQuotedTableName_WhenTableIsSet()
    {
        // Arrange
        _sut.SetTable("student");
        _quoter.Quote("student").Returns("\"student\"");
        var expected = "FROM \"student\"";

        // Act
        var output = _sut.Render(_quoter, _placeholders, Array.Empty<object?>());

        // Assert
        output.Sql.Should().Be(expected);
    }
}
