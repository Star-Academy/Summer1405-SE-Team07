using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using QueryLib.Compilers;
using QueryLib.Compilers.Abstractions;
using Xunit;

namespace QueryLib.IntegrationTest.SqlServer;

public class SqlserverIntegrationTests : IClassFixture<SqlServerFixture>, IAsyncLifetime
{
    private readonly SqlServerFixture _sqlServerFixture;
    private ICompiler _sut = null!;
    private SqlConnection _connection = null!;

    public SqlserverIntegrationTests(SqlServerFixture fixture)
    {
        _sqlServerFixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _sut = _sqlServerFixture.ServiceProvider!.GetRequiredService<ICompiler>();
        _connection = new SqlConnection(_sqlServerFixture.ConnectionString);
        await _connection.OpenAsync();
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task Compile_ShouldReturnAllRows_WhenSelectHasNoColumnsSpecified()
    {
        // Arrange
        var query = new Query()
            .From("Student")
            .Select();

        // Act
        var compiledQuery = _sut.Compile(query, DbProvider.SqlServer);
        var rows = await ExecuteCompiledQueryAsync(compiledQuery);

        // Assert
        rows.Should().HaveCount(20);
        rows.First().Keys.Should().Contain(new[]
        {
            "StudentNumber", "Grade", "FirstName", "LastName", "IsMale", "DateOfBirth", "LeftUnitsCount"
        });
    }

    [Fact]
    public async Task Compile_ShouldReturnProjectedColumns_WhenSpecificColumnsSelected()
    {
        // Arrange
        var query = new Query()
            .From("Student")
            .Select("FirstName", "LastName");

        // Act
        var compiledQuery = _sut.Compile(query, DbProvider.SqlServer);
        var rows = await ExecuteCompiledQueryAsync(compiledQuery);

        // Assert
        rows.Should().HaveCount(20);
        foreach (var row in rows)
        {
            row.Keys.Should().BeEquivalentTo(new[] { "FirstName", "LastName" });
            row["FirstName"].Should().NotBeNull();
            row["LastName"].Should().NotBeNull();
        }
    }

    [Fact]
    public async Task Compile_ShouldReturnFilteredRows_WhenSingleConditionWhereApplied()
    {
        // Arrange
        var query = new Query()
            .From("Student")
            .Select("FirstName", "LastName", "IsMale")
            .Where("IsMale", true);

        // Act
        var compiledQuery = _sut.Compile(query, DbProvider.SqlServer);
        var rows = await ExecuteCompiledQueryAsync(compiledQuery);

        // Assert
        rows.Should().HaveCount(11);
        rows.Should().OnlyContain(r => (bool)r["IsMale"]! == true);
        rows.Select(r => (string)r["FirstName"]!).Should().Contain(new[] { "John", "Michael", "William" });
        rows.Select(r => (string)r["FirstName"]!).Should().NotContain(new[] { "Emily", "Sarah", "Olivia" });
    }

    [Fact]
    public async Task Compile_ShouldReturnFilteredRows_WhenMultipleWhereConditionsApplied()
    {
        // Arrange
        var query = new Query()
            .From("Student")
            .Select("StudentNumber", "FirstName", "LastName", "IsMale", "LeftUnitsCount")
            .Where("IsMale", true)
            .Where("LeftUnitsCount", 92);

        // Act
        var compiledQuery = _sut.Compile(query, DbProvider.SqlServer);
        var rows = await ExecuteCompiledQueryAsync(compiledQuery);

        // Assert
        rows.Should().HaveCount(1);
        var student = rows.Single();
        student["StudentNumber"].Should().Be("98100200");
        student["FirstName"].Should().Be("John");
        student["LastName"].Should().Be("Smith");
    }

    [Fact]
    public async Task Compile_ShouldReturnEmptyList_WhenNoRecordsMatchWhereCondition()
    {
        // Arrange
        var query = new Query()
            .From("Student")
            .Select("FirstName", "LastName")
            .Where("FirstName", "NonExistentStudentName");

        // Act
        var compiledQuery = _sut.Compile(query, DbProvider.SqlServer);
        var rows = await ExecuteCompiledQueryAsync(compiledQuery);

        // Assert
        rows.Should().BeEmpty();
    }

    private async Task<List<Dictionary<string, object?>>> ExecuteCompiledQueryAsync(CompiledQuery compiledQuery)
    {
        await using var command = new SqlCommand(compiledQuery.Sql, _connection);
        for (int param = 0; param < compiledQuery.Bindings.Count; param++)
        {
            command.Parameters.Add(new SqlParameter($"@p{param}", compiledQuery.Bindings[param] ?? DBNull.Value));
        }

        await using var reader = await command.ExecuteReaderAsync();
        var rows = new List<Dictionary<string, object?>>();

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            rows.Add(row);
        }

        return rows;
    }
}
