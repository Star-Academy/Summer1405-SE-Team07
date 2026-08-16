using FluentAssertions;
using QueryLib.Compilers;

namespace QueryLib.Tests.Compilers;

public class SqlCompilerFactoryTests
{
    private readonly SqlCompilerFactory _sut;

    public SqlCompilerFactoryTests()
    {
        _sut = new SqlCompilerFactory();
    }

    [Fact]
    public void Create_ShouldReturnPostgresCompiler_WhenCompilerTypeIsPostgres()
    {
        // Arrange
        const string compilerType = "postgres";
        var query = new Query()
            .From("student")
            .Select("id", "name")
            .Where("id", 10);
        var expected = new CompiledQuery("SELECT \"id\", \"name\" FROM \"student\" WHERE \"id\" = $1" ,
            new object?[] { 10 });  
        
        // Act
        var compiler = _sut.Create(compilerType);
        var result = compiler.Compile(query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Create_ShouldReturnSqlServerCompiler_WhenCompilerTypeIsSqlServer()
    {
        // Arrange
        const string compilerType = "sqlserver";
        var query = new Query()
            .From("student")
            .Select("id", "name")
            .Where("id", 10);
        var expected = new CompiledQuery("SELECT [id], [name] FROM [student] WHERE [id] = @p0" ,
            new object?[] { 10 });  
        
        //Act
        var compiler = _sut.Create(compilerType);
        var result = compiler.Compile(query);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
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
        // Arrange
        var unsupportedCompilerType = compilerType!;

        // Act
        var act = () => _sut.Create(unsupportedCompilerType);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }
}
