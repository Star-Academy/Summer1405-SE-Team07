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
    public void DbType_ShouldBeSqlServer_WhenAccessed()
    {
        // Arrange
        const DbProvider expected = DbProvider.SqlServer;

        // Act
        var dbType = _sut.DbType;

        // Assert
        dbType.Should().Be(expected);
    }

    [Fact]
    public void Bind_ShouldConvertTrueToOne_WhenCalled()
    {
        // Arrange 
        
        //Act
        var result = _sut.Bind(true);
        
        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void Bind_ShouldConvertFalseToZero_WhenCalled()
    {
        // Arrange

        // Act
        var result = _sut.Bind(false);
        
        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData("test")]
    [InlineData(42)]
    [InlineData(null)]
    public void Bind_ShouldReturnNonBooleanValuesAsIs_WhenCalled(object? value)
    {
        // Arrange 
        
        // Act
        var result = _sut.Bind(value);
        
        // Assert
        result.Should().Be(value);
    }
}
