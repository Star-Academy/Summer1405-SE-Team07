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
        var provider = _sut.Provider;

        // Assert
        provider.Should().Be(expected);
    }

    [Fact]
    public void MakePlaceholder_ShouldReturnDollarSignFollowedByIndex_Whenever()
    {
        // Arrange
        const int index = 1;
        const string expected = "$1";
        
        // Act
        var result = _sut.MakePlaceholder(index);
        
        // Assert
        result.Should().Be(expected);
    }
}
