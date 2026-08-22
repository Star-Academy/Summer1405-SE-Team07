using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Compilers;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers;
using QueryLib.Renderers.Abstractions;

namespace QueryBuilder.Test.Compilers;

public class SqlCompilerTests
{
    private readonly IValueBinder _binder = Substitute.For<IValueBinder>();
    private readonly IClauseRenderer _selectRenderer = Substitute.For<IClauseRenderer>();
    private readonly IClauseRenderer _fromRenderer = Substitute.For<IClauseRenderer>();

    public SqlCompilerTests()
    {
        _selectRenderer.ClauseType.Returns(typeof(SelectClause));
        _fromRenderer.ClauseType.Returns(typeof(FromClause));
    }

    [Fact]
    public void Compile_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Arrange
        var sut = new SqlCompiler(_binder, new ClauseRendererRegistry([_selectRenderer, _fromRenderer]));
        Query? query = null;

        // Act
        var act = () => sut.Compile(query!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("query");
    }

    [Fact]
    public void Compile_ShouldJoinRenderedClausesInOrderAndBindValues_WhenClausesProduceSql()
    {
        // Arrange
        var query = new Query().From("student").Select("id");
        _selectRenderer.Render(Arg.Any<IQueryClause>(), Arg.Any<IReadOnlyCollection<object?>>())
            .Returns(new RenderOutput("SELECT *", new object?[] { "raw" }));
        _fromRenderer.Render(Arg.Any<IQueryClause>(), Arg.Any<IReadOnlyCollection<object?>>())
            .Returns(new RenderOutput("FROM \"student\"", new object?[] { "raw" }));
        _binder.Bind("raw").Returns("bound");
        var sut = new SqlCompiler(_binder, new ClauseRendererRegistry([_selectRenderer, _fromRenderer]));
        var expected = new CompiledQuery("SELECT * FROM \"student\"", new object?[] { "bound", "bound" });

        // Act
        var result = sut.Compile(query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Compile_ShouldOmitBlankSqlParts_WhenARendererReturnsWhitespaceSql()
    {
        // Arrange
        var query = new Query().From("student").Select("id");
        _selectRenderer.Render(Arg.Any<IQueryClause>(), Arg.Any<IReadOnlyCollection<object?>>())
            .Returns(new RenderOutput(string.Empty, Array.Empty<object?>()));
        _fromRenderer.Render(Arg.Any<IQueryClause>(), Arg.Any<IReadOnlyCollection<object?>>())
            .Returns(new RenderOutput("FROM \"student\"", Array.Empty<object?>()));
        var sut = new SqlCompiler(_binder, new ClauseRendererRegistry([_selectRenderer, _fromRenderer]));
        var expected = new CompiledQuery("FROM \"student\"", new List<object?>());

        // Act
        var result = sut.Compile(query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}