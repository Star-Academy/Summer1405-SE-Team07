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
    public void Provider_ShouldBePostgreSql()
    {
        // Arrange
        const DbProvider expected = DbProvider.PostgreSql;

        // Act
        var provider = _sut.Provider;

        // Assert
        provider.Should().Be(expected);
    }

    [Fact]
    public void Quote_ShouldWrapIdentifierInDoubleQuotes()
    {
        // Arrange
        const string expected = "\"column_name\"";

        // Act
        var result = _sut.Quote("column_name");

        // Assert
        result.Should().Be(expected);
    }
}

