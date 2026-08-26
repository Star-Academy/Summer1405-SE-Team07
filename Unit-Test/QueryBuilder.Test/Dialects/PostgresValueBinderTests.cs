using FluentAssertions;
using QueryLib;
using QueryLib.Dialects.Postgres;

namespace QueryBuilder.Test.Dialects;

public class PostgresValueBinderTests
{
    private readonly PostgresValueBinder _sut;

    public PostgresValueBinderTests()
    {
        _sut = new PostgresValueBinder();
    }

    [Fact]
    public void DbType_ShouldBePostgreSql_Whenever()
    {
        // Arrange
        const DbProvider expected = DbProvider.PostgreSql;

        // Act
        var actual = _sut.DbType;

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("test")]
    [InlineData(42)]
    [InlineData(true)]
    [InlineData(null)]
    public void Bind_ShouldReturnValueAsIs_Whenever(object? value)
    {
        // Arrange 

        // Act
        var actual = _sut.Bind(value);
        
        // Assert
        actual.Should().Be(value);
    }
}
