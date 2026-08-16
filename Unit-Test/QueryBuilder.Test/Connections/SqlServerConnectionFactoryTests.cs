
using FluentAssertions;
using Microsoft.Data.SqlClient;
using QueryLib.Demo.Connections;


namespace QueryLib.Demo.Tests.Connections;

public class SqlServerConnectionFactoryTests
{
    private const string ConnectionString =
        "Server=v1,1433;Database=v2;User Id=sa;Password=123;Encrypt=False;TrustServerCertificate=True";

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenConnectionStringIsNull()
    {
        // Arrange
        string? connectionString = null;

        // Act
        var act = () => new SqlServerConnectionFactory(connectionString!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("connectionString");
    }

    [Fact]
    public async Task CreateConnectionAsync_ShouldReturnSqlConnection_WhenCalled()
    {
        // Arrange
        var factory = new SqlServerConnectionFactory(ConnectionString);

        // Act
        await using var connection = await factory.CreateConnectionAsync();

        // Assert
        connection.Should().NotBeNull();
        connection.Should().BeOfType<SqlConnection>();
    }

    [Fact]
    public async Task CreateConnectionAsync_ShouldUseConfiguredConnectionString_WhenCalled()
    {
        // Arrange
        var factory = new SqlServerConnectionFactory(ConnectionString);

        // Act
        await using var connection = await factory.CreateConnectionAsync();

        // Assert
        connection.ConnectionString.Should().Be(ConnectionString);
    }
}
