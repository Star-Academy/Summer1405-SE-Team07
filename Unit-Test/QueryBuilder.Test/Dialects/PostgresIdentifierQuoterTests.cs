using FluentAssertions;
using QueryLib;
using QueryLib.Dialects.Postgres;

namespace QueryBuilder.Test.Dialects;

public class PostgresIdentifierQuoterTests
{
    private readonly PostgresIdentifierQuoter _sut;

    public PostgresIdentifierQuoterTests()
    {
        _sut = new PostgresIdentifierQuoter();
    }

    [Fact]
    public void Provider_ShouldBePostgreSql_Whenever()
    {
        // Arrange
        const DbProvider expected = DbProvider.PostgreSql;

        // Act
        var actual = _sut.Provider;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Quote_ShouldWrapIdentifierInDoubleQuotes_Whenever()
    {
        // Arrange
        const string identifier = "column_name";
        const string expected = "\"" + identifier + "\"";

        // Act
        var actual = _sut.Quote(identifier);

        // Assert
        actual.Should().Be(expected);
    }
}
