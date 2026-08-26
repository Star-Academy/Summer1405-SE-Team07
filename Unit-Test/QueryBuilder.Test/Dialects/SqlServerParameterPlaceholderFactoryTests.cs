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
    public void MakePlaceholder_ShouldReturnParameterPlaceholderWithZeroBasedIndex_Whenever()
    {
        // Arrange 
        const int index = 10;
        const string expected = "@p9";
        
        // Act
        var actual = _sut.MakePlaceholder(index);
        
        // Assert
        actual.Should().Be(expected);
    }
}
