
using FluentAssertions;
using Microsoft.Data.SqlClient;
using QueryLib.Demo.Connections;


namespace QueryLib.Demo.Tests.Connections;

public class SqlServerConnectionFactoryTests
{
    private const string ConnectionString =
        "Server=v1,1433;Database=v2;User Id=sa;Password=123;Encrypt=False;TrustServerCertificate=True";

    private readonly SqlServerConnectionFactory _sut;

    public SqlServerConnectionFactoryTests()
    {
        _sut = new SqlServerConnectionFactory(ConnectionString);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenConnectionStringIsNull()
    {
        //Arrange
        var expected = "connectionString";
        
        // Act
        var act = () => new SqlServerConnectionFactory(null!);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName(expected);
    }

    [Fact]
    public async Task CreateConnectionAsync_ShouldReturnSqlConnection_WhenCalled()
    {
        // Arrange
        
        // Act
        await using var connection = await _sut.CreateConnectionAsync();

        // Assert
        connection.Should().BeOfType<SqlConnection>();
    }

    [Fact]
    public async Task CreateConnectionAsync_ShouldUseConfiguredConnectionString_WhenCalled()
    {
        // Arrange
        
        // Act
        await using var connection = await _sut.CreateConnectionAsync();

        // Assert
        connection.ConnectionString.Should().Be(ConnectionString);
    }
}
