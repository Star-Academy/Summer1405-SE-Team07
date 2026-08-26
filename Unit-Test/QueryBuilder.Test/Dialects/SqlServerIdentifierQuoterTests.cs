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
        var provider = _sut.Provider;

        // Assert
        provider.Should().Be(expected);
    }

    [Fact]
    public void Quote_ShouldWrapIdentifierInBrackets_Whenever()
    {
        // Arrange
        const string expected = "[column_name]";

        // Act
        var result = _sut.Quote("column_name");

        // Assert
        result.Should().Be(expected);
    }
}
