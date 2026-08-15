using FluentAssertions;
using QueryLib.Compilers;

namespace QueryLib.Tests.Compilers;

public class SqlCompilerFactoryTests{private readonly SqlCompilerFactory _sut;
    public SqlCompilerFactoryTests()
    {
        _sut = new SqlCompilerFactory();
    }

    [Fact]
    public void Create_ShouldReturnPostgresCompiler_WhenCompilerTypeIsPostgres()
    {
        // Arrange
        const string compilerType = "postgres";

        // Act
        var compiler = _sut.Create(compilerType);

        // Assert
        compiler.Should().NotBeNull();

        var query = new Query()
            .From("student")
            .Select("id", "name")
            .Where("id", 10);

        var result = compiler.Compile(query);

        result.Sql.Should()
            .Be("SELECT \"id\", \"name\" FROM \"student\" WHERE \"id\" = $1");

        result.Bindings
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(10);
    }

    [Fact]
    public void Create_ShouldReturnSqlServerCompiler_WhenCompilerTypeIsSqlServer()
    {
        // Arrange
        const string compilerType = "sqlserver";

        // Act
        var compiler = _sut.Create(compilerType);

        // Assert
        compiler.Should().NotBeNull();

        var query = new Query()
            .From("student")
            .Select("id", "name")
            .Where("id", 10);

        var result = compiler.Compile(query);

        result.Sql.Should()
            .Be("SELECT [id], [name] FROM [student] WHERE [id] = @p0");

        result.Bindings
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(10);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("mysql")]
    [InlineData("oracle")]
    [InlineData("unknown")]
    public void Create_ShouldThrowNotImplementedException_WhenCompilerTypeIsUnsupported(
        string? compilerType)
    {
        // Act
        var act = () => _sut.Create(compilerType!);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }


}
