using System.Data.Common;
using FluentAssertions;
using Npgsql;
using QueryLib.Demo.Connections;

namespace QueryLib.Demo.Tests.Connections;

public class NpgsqlConnectionFactoryTests
{
    private readonly NpgsqlConnectionFactory _sut;

    public NpgsqlConnectionFactoryTests()
    {
        //Arrange
        _sut = new NpgsqlConnectionFactory();
    }

    [Fact]
    public void Create_ShouldReturnNpgsqlConnection_WhenCalled()
    {
        // Arrange
        var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=test";
        
        // Act
        using var connection = _sut.Create(connectionString);

        // Assert
        connection.Should().BeOfType<NpgsqlConnection>();
        connection.ConnectionString.Should().Be(connectionString);
    }
}
