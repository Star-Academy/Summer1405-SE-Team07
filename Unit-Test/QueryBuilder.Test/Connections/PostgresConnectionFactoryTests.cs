using FluentAssertions;
using Npgsql;
using QueryLib.Demo.Connections;

namespace QueryLib.Demo.Tests.Connections;

public class PostgresConnectionFactoryTests
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=test";

    [Fact]
    public void Constructor_WhenConnectionStringIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        string? connectionString = null;

        // Act
        var act = () => new PostgresConnectionFactory(connectionString!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("connectionString");
    }

    [Fact]
    public async Task CreateConnectionAsync_WhenCalled_ShouldReturnNpgsqlConnection()
    {
        // Arrange
        var factory = new PostgresConnectionFactory(ConnectionString);

        // Act
        await using var connection = await factory.CreateConnectionAsync();

        // Assert
        connection.Should().NotBeNull();
        connection.Should().BeOfType<NpgsqlConnection>();
    }

    [Fact]
    public async Task CreateConnectionAsync_WhenCalled_ShouldUseConfiguredConnectionString()
    {
        // Arrange
        var factory = new PostgresConnectionFactory(ConnectionString);

        // Act
        await using var connection = await factory.CreateConnectionAsync();

        // Assert
        connection.ConnectionString.Should().Be(ConnectionString);
    }
}
