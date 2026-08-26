using FluentAssertions;
using QueryLib;
using QueryLib.Dialects.SqlServer;

namespace QueryBuilder.Test.Dialects;

public class SqlServerIdentifierQuoterTests
{
    private readonly SqlServerIdentifierQuoter _sut;

    public SqlServerIdentifierQuoterTests()
    {
        _sut = new SqlServerIdentifierQuoter();
    }

    [Fact]
    public void Provider_ShouldBeSqlServer_Whenever()
    {
        // Arrange
        const DbProvider expected = DbProvider.SqlServer;

        // Act
        var actual = _sut.Provider;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Quote_ShouldWrapIdentifierInBrackets_Whenever()
    {
        // Arrange
        const string identifier = "column_name";
        const string expected = "[" + identifier + "]";

        // Act
        var actual = _sut.Quote("column_name");

        // Assert
        actual.Should().Be(expected);
    }
}
