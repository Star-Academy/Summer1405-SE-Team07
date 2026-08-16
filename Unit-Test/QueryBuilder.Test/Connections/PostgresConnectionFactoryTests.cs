using FluentAssertions;
using Npgsql;
using QueryLib.Demo.Connections;

namespace QueryLib.Demo.Tests.Connections;

public class PostgresConnectionFactoryTests
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=test";

    private readonly PostgresConnectionFactory _sut;

    public PostgresConnectionFactoryTests()
    {
        //Arrange
        _sut = new PostgresConnectionFactory(ConnectionString);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenConnectionStringIsNull()
    {
        // Arrange
        var expected = "connectionString";
        
        // Act
        var act = () => new PostgresConnectionFactory(null!);
        
        

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName(expected);
    }

    [Fact]
    public async Task CreateConnectionAsync_ShouldReturnNpgsqlConnection_WhenCalled()
    {
        
        
        // Act
        await using var connection = await _sut.CreateConnectionAsync();

        // Assert
        connection.Should().BeOfType<NpgsqlConnection>();
    }

    [Fact]
    public async Task CreateConnectionAsync_ShouldUseConfiguredConnectionString_WhenCalled()
    {
        // Act
        await using var connection = await _sut.CreateConnectionAsync();

        // Assert
        connection.ConnectionString.Should().Be(ConnectionString);
    }
}
