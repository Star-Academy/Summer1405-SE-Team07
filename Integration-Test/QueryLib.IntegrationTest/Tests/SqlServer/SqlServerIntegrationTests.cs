using FluentAssertions;
using QueryLib.Compilers.Abstractions;
using QueryLib.IntegrationTest.Infrastructure.SqlServer;
using Xunit;

namespace QueryLib.IntegrationTest.Tests.SqlServer;

public class SqlServerIntegrationTests : IClassFixture<SqlServerFixture>, IAsyncLifetime
{
    private readonly SqlServerFixture _sqlServerFixture;
    private ICompiler _sut = null!;
    private SqlServerCompiledQueryExecutor _executor = null!;

    public SqlServerIntegrationTests(SqlServerFixture fixture)
    {
        _sqlServerFixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _sut = _sqlServerFixture.Compiler;
        _executor = new SqlServerCompiledQueryExecutor(_sqlServerFixture.ConnectionString);
        await _executor.OpenAsync();
    }

    public async Task DisposeAsync()
    {
        await _executor.DisposeAsync();
    }

    [Fact]
    public async Task Compile_ShouldReturnAllRows_WhenSelectHasNoColumnsSpecified()
    {
        var query = new Query()
            .From("Student")
            .Select();

        var compiledQuery = _sut.Compile(query, DbProvider.SqlServer);
        var rows = await _executor.ExecuteAsync(compiledQuery);

        var expectedRowCount = 20;
        var expected = new[]
        {
            "StudentNumber", "Grade", "FirstName", "LastName", "IsMale", "DateOfBirth", "LeftUnitsCount"
        };

        rows.Should().HaveCount(expectedRowCount);
        rows.First().Keys.Should().Contain(expected);
    }

    [Fact]
    public async Task Compile_ShouldReturnProjectedColumns_WhenSpecificColumnsSelected()
    {
        var query = new Query()
            .From("Student")
            .Select("FirstName", "LastName");

        var compiledQuery = _sut.Compile(query, DbProvider.SqlServer);
        var rows = await _executor.ExecuteAsync(compiledQuery);

        var expectedRowCount = 20;
        var expected = new[] { "FirstName", "LastName" };

        rows.Should().HaveCount(expectedRowCount);
        foreach (var row in rows)
        {
            row.Keys.Should().BeEquivalentTo(expected);
            row["FirstName"].Should().NotBeNull();
            row["LastName"].Should().NotBeNull();
        }
    }

    [Fact]
    public async Task Compile_ShouldReturnFilteredRows_WhenSingleConditionWhereApplied()
    {
        var query = new Query()
            .From("Student")
            .Select("FirstName", "LastName", "IsMale")
            .Where("IsMale", true);

        var compiledQuery = _sut.Compile(query, DbProvider.SqlServer);
        var rows = await _executor.ExecuteAsync(compiledQuery);

        var expectedRowCount = 11;
        var expectedFirstNames = new[] { "John", "Michael", "William" };
        var unexpectedFirstNames = new[] { "Emily", "Sarah", "Olivia" };

        rows.Should().HaveCount(expectedRowCount);
        rows.Should().OnlyContain(r => (bool)r["IsMale"]! == true);
        rows.Select(r => (string)r["FirstName"]!).Should().Contain(expectedFirstNames);
        rows.Select(r => (string)r["FirstName"]!).Should().NotContain(unexpectedFirstNames);
    }

    [Fact]
    public async Task Compile_ShouldReturnFilteredRows_WhenMultipleWhereConditionsApplied()
    {
        var query = new Query()
            .From("Student")
            .Select("StudentNumber", "FirstName", "LastName", "IsMale", "LeftUnitsCount")
            .Where("IsMale", true)
            .Where("LeftUnitsCount", 92);

        var compiledQuery = _sut.Compile(query, DbProvider.SqlServer);
        var rows = await _executor.ExecuteAsync(compiledQuery);

        var expectedRowCount = 1;
        var expectedStudentNumber = "98100200";
        var expectedFirstName = "John";
        var expectedLastName = "Smith";

        rows.Should().HaveCount(expectedRowCount);
        var student = rows.Single();
        student["StudentNumber"].Should().Be(expectedStudentNumber);
        student["FirstName"].Should().Be(expectedFirstName);
        student["LastName"].Should().Be(expectedLastName);
    }

    [Fact]
    public async Task Compile_ShouldReturnEmptyList_WhenNoRecordsMatchWhereCondition()
    {
        var query = new Query()
            .From("Student")
            .Select("FirstName", "LastName")
            .Where("FirstName", "NonExistentStudentName");

        var compiledQuery = _sut.Compile(query, DbProvider.SqlServer);
        var rows = await _executor.ExecuteAsync(compiledQuery);

        rows.Should().BeEmpty();
    }
}
