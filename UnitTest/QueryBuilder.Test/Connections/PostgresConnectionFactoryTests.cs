using FluentAssertions;
using Npgsql;
using QueryLib.Demo.Connections;
using Xunit;

namespace QueryLib.Demo.Tests.Connections;

public class PostgresConnectionFactoryTests
{
    private readonly PostgresConnectionFactory _sut;

    public PostgresConnectionFactoryTests()
    {
        _sut = new PostgresConnectionFactory(
            "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=test");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenConnectionStringIsNull()
    {
        // Arrange 
        var act = () => new PostgresConnectionFactory(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("connectionString");
    }

    [Fact]
    public async Task CreateConnectionAsync_ShouldReturnNpgsqlConnection()
    {
        // Act
        await using var connection = await _sut.CreateConnectionAsync();

        // Assert
        connection.Should().NotBeNull();
        connection.Should().BeOfType<NpgsqlConnection>();
    }

    [Fact]
    public async Task CreateConnectionAsync_ShouldUseConfiguredConnectionString()
    {
        // Act
        await using var connection = await _sut.CreateConnectionAsync();

        // Assert
        connection.ConnectionString.Should().Be("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=test");
    }
}