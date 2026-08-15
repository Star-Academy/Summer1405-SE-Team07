
using FluentAssertions;
using Microsoft.Data.SqlClient;
using QueryLib.Demo.Connections;


namespace QueryLib.Demo.Tests.Connections;

public class SqlServerConnectionFactoryTests
{
    private readonly SqlServerConnectionFactory _sut;

    public SqlServerConnectionFactoryTests()
    {
        _sut = new SqlServerConnectionFactory(
            "Server=v1,1433;Database=v2;User Id=sa;Password=123;Encrypt=False;TrustServerCertificate=True");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenConnectionStringIsNull()
    {
        // Arrange 
        var act = () => new SqlServerConnectionFactory(null!);
        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("connectionString");
    }

    [Fact]
    public async Task CreateConnectionAsync_ShouldReturnsqlserverConnection()
    {
        // Act
        await using var connection = await _sut.CreateConnectionAsync();
        // Assert
        connection.Should().NotBeNull();
        connection.Should().BeOfType<SqlConnection>();
    }

    [Fact]
    public async Task CreateConnectionAsync_ShouldUseConfiguredConnectionString()
    {
        // Act
        await using var connection = await _sut.CreateConnectionAsync();
        // Assert
        connection.ConnectionString.Should().Be("Server=v1,1433;Database=v2;User Id=sa;Password=123;Encrypt=False;TrustServerCertificate=True");
    }
} 