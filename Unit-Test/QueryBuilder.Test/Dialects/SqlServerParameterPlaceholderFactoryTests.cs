using FluentAssertions;
using QueryLib;
using QueryLib.Dialects.SqlServer;

namespace QueryBuilder.Test.Dialects;

public class SqlServerParameterPlaceholderFactoryTests
{
    private readonly SqlServerParameterPlaceholderFactory _sut;

    public SqlServerParameterPlaceholderFactoryTests()
    {
        _sut = new SqlServerParameterPlaceholderFactory();
    }

    [Fact]
    public void Provider_ShouldBeSqlServer_WhenAccessed()
    {
        // Arrange
        const DbProvider expected = DbProvider.SqlServer;

        // Act
        var provider = _sut.Provider;

        // Assert
        provider.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, "@p0")]
    [InlineData(2, "@p1")]
    [InlineData(10, "@p9")]
    public void MakePlaceholder_ShouldReturnAtPWithZeroBasedIndex_WhenCalled(int index, string expected)
    {
        // Arrange 
        
        // Act
        var result = _sut.MakePlaceholder(index);
        
        // Assert
        result.Should().Be(expected);
    }
}
