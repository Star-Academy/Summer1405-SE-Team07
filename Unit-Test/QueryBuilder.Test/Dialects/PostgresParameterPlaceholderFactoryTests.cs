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
    public void Provider_ShouldBePostgreSql_WhenAccessed()
    {
        // Arrange
        const DbProvider expected = DbProvider.PostgreSql;

        // Act
        var provider = _sut.Provider;

        // Assert
        provider.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, "$1")]
    [InlineData(2, "$2")]
    [InlineData(10, "$10")]
    public void MakePlaceholder_ShouldReturnDollarSignFollowedByIndex_WhenCalled(int index, string expected)
    {
        // Arrange

        // Act
        var result = _sut.MakePlaceholder(index);
        
        // Assert
        result.Should().Be(expected);
    }
}
