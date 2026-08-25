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
    private readonly IValueBinderFactory _valueBinderFactory = Substitute.For<IValueBinderFactory>();
    private readonly IClauseRendererRegistryFactory _registryFactory = Substitute.For<IClauseRendererRegistryFactory>();
    private readonly IValueBinder _binder = Substitute.For<IValueBinder>();
    private readonly IClauseRenderer _selectRenderer = Substitute.For<IClauseRenderer>();
    private readonly IClauseRenderer _fromRenderer = Substitute.For<IClauseRenderer>();
    private readonly SqlCompiler _sut;

    public SqlCompilerTests()
    {
        _selectRenderer.ClauseKind.Returns(ClauseKind.Select);
        _fromRenderer.ClauseKind.Returns(ClauseKind.From);

        var registry = new ClauseRendererRegistry(DbProvider.PostgreSql, [_selectRenderer, _fromRenderer]);
        _registryFactory.GetRegistry(DbProvider.PostgreSql).Returns(registry);
        _valueBinderFactory.GetBinder(DbProvider.PostgreSql).Returns(_binder);

        _sut = new SqlCompiler(_valueBinderFactory, _registryFactory);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenValueBinderFactoryIsNull()
    {
        // Arrange
        var act = () => new SqlCompiler(null!, _registryFactory);

        // Act
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("valueBinderFactory");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenRegistryFactoryIsNull()
    {
        // Arrange
        var act = () => new SqlCompiler(_valueBinderFactory, null!);

        // Act
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("registryFactory");
    }

    [Fact]
    public void Compile_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Arrange
        var act = () => _sut.Compile(null!, DbProvider.PostgreSql);

        // Act
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("query");
    }

    [Fact]
    public void Compile_ShouldJoinRenderedClausesInOrderAndBindValues_WhenClausesProduceSql()
    {
        // Arrange
        var query = new Query().From("student").Select("id");
        _selectRenderer.Render(Arg.Any<IQueryClause>())
            .Returns(new RenderOutput("SELECT \"id\"", new object?[] { "raw-select" }));
        _fromRenderer.Render(Arg.Any<IQueryClause>())
            .Returns(new RenderOutput("FROM \"student\"", new object?[] { "raw-from" }));
        _binder.Bind("raw-select").Returns("bound-select");
        _binder.Bind("raw-from").Returns("bound-from");

        var expected = new CompiledQuery("SELECT \"id\" FROM \"student\"", new object?[] { "bound-select", "bound-from" });

        // Act
        var result = _sut.Compile(query, DbProvider.PostgreSql);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Compile_ShouldOmitBlankSqlParts_WhenARendererReturnsWhitespaceSql()
    {
        // Arrange
        var query = new Query().From("student").Select("id");
        _selectRenderer.Render(Arg.Any<IQueryClause>())
            .Returns(new RenderOutput(string.Empty, Array.Empty<object?>()));
        _fromRenderer.Render(Arg.Any<IQueryClause>())
            .Returns(new RenderOutput("FROM \"student\"", Array.Empty<object?>()));

        var expected = new CompiledQuery("FROM \"student\"", new List<object?>());

        // Act
        var result = _sut.Compile(query, DbProvider.PostgreSql);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
