using FluentAssertions;
using QueryLib;
using QueryLib.Dialects.Postgres;

namespace QueryBuilder.Test.Dialects;

public class PostgresParameterPlaceholderFactoryTests
{
    private readonly PostgresParameterPlaceholderFactory _sut;

    public PostgresParameterPlaceholderFactoryTests()
    {
        _sut = new PostgresParameterPlaceholderFactory();
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
    public void MakePlaceholder_ShouldReturnDollarSignFollowedByIndex_Whenever()
    {
        // Arrange
        const int index = 1;
        const string expected = "$1";
        
        // Act
        var actual = _sut.MakePlaceholder(index);
        
        // Assert
        actual.Should().Be(expected);
    }
}
