using FluentAssertions;
using QueryLib;
using QueryLib.Dialects.SqlServer;

namespace QueryBuilder.Test.Dialects;

public class SqlServerValueBinderTests
{
    private readonly SqlServerValueBinder _sut;

    public SqlServerValueBinderTests()
    {
        _sut = new SqlServerValueBinder();
    }

    [Fact]
    public void DbType_ShouldBeSqlServer_Whenever()
    {
        // Arrange
        const DbProvider expected = DbProvider.SqlServer;

        // Act
        var actual = _sut.DbType;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Bind_ShouldConvertTrueToOne_Whenever()
    {
        // Arrange 
        
        //Act
        var actual = _sut.Bind(true);
        
        // Assert
        actual.Should().Be(1);
    }

    [Fact]
    public void Bind_ShouldConvertFalseToZero_Whenever()
    {
        // Arrange

        // Act
        var actual = _sut.Bind(false);
        
        // Assert
        actual.Should().Be(0);
    }

    [Theory]
    [InlineData("test")]
    [InlineData(42)]
    [InlineData(null)]
    public void Bind_ShouldReturnNonBooleanValuesAsIs_Whenever(object? value)
    {
        // Arrange 
        
        // Act
        var actual = _sut.Bind(value);
        
        // Assert
        actual.Should().Be(value);
    }
}
