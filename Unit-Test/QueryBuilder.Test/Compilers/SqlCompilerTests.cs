using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Clauses.Abstractions;
using QueryLib.Compilers;
using QueryLib.Dialects.Abstractions;

namespace QueryBuilder.Test.Compilers;

public sealed class SqlCompilerTests
{
    private readonly IIdentifierQuoter _quoter = Substitute.For<IIdentifierQuoter>();
    private readonly IParameterPlaceholderFactory _placeholders =
        Substitute.For<IParameterPlaceholderFactory>();
    private readonly IValueBinder _binder = Substitute.For<IValueBinder>();
    private readonly SqlCompiler _sut;

    public SqlCompilerTests()
    {
        _sut = new SqlCompiler(_quoter, _placeholders, _binder);
    }

    [Fact]
    public void Constructor_WhenQuoterIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IIdentifierQuoter? quoter = null;

        // Act
        var act = () => new SqlCompiler(quoter!, _placeholders, _binder);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("quoter");
    }

    [Fact]
    public void Constructor_WhenPlaceholderFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IParameterPlaceholderFactory? placeholders = null;

        // Act
        var act = () => new SqlCompiler(_quoter, placeholders!, _binder);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("placeholders");
    }

    [Fact]
    public void Constructor_WhenBinderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IValueBinder? binder = null;

        // Act
        var act = () => new SqlCompiler(_quoter, _placeholders, binder!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("binder");
    }

    [Fact]
    public void Compile_WhenQueryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Query? query = null;

        // Act
        var act = () => _sut.Compile(query!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("query");
    }

    [Fact]
    public void Compile_WhenFromClauseIsMissing_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var query = new Query();

        // Act
        var act = () => _sut.Compile(query);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("From(...) must be called before compiling the query.");
    }

    [Fact]
    public void Compile_WhenNoColumnsAreSelected_ShouldUseWildcardAndReturnNoBindings()
    {
        // Arrange
        var query = new Query().From("student");
        _quoter.Quote("student").Returns("\"student\"");

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Sql.Should().Be("SELECT * FROM \"student\"");
        result.Bindings.Should().BeEmpty();
        _binder.DidNotReceive().Bind(Arg.Any<object?>());
    }

    [Fact]
    public void Compile_WhenColumnsAndConditionsExist_ShouldCompileSqlAndBindValues()
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("id", "name")
            .Where("id", 10)
            .Where("status", "active");

        _quoter.Quote(Arg.Any<string>())
            .Returns(call => $"\"{call.Arg<string>()}\"");
        _placeholders.MakePlaceholder(1).Returns("@p1");
        _placeholders.MakePlaceholder(2).Returns("@p2");
        _binder.Bind(10).Returns("bound-id");
        _binder.Bind("active").Returns("bound-status");

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Sql.Should().Be(
            "SELECT \"id\", \"name\" FROM \"student\" " +
            "WHERE \"id\" = @p1 AND \"status\" = @p2");
        result.Bindings.Should().Equal("bound-id", "bound-status");
    }

    [Fact]
    public void Compile_WhenClausesAreAddedOutOfOrder_ShouldRenderByClauseOrder()
    {
        // Arrange
        var query = new Query().From("student");
        var orderByClause = CreateClause(30, "ORDER BY name");
        var whereClause = CreateClause(20, "WHERE age > @p1");
        _quoter.Quote("student").Returns("\"student\"");

        query.AddClause(orderByClause);
        query.AddClause(whereClause);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Sql.Should().Be(
            "SELECT * FROM \"student\" WHERE age > @p1 ORDER BY name");
    }

    [Fact]
    public void Compile_WhenMultipleClausesProduceBindings_ShouldPassBindingsBetweenClauses()
    {
        // Arrange
        var query = new Query().From("student");
        var firstClause = Substitute.For<IQueryClause>();
        var secondClause = Substitute.For<IQueryClause>();
        firstClause.Order.Returns(20);
        secondClause.Order.Returns(30);
        _quoter.Quote("student").Returns("\"student\"");

        firstClause.Render(
                _quoter,
                _placeholders,
                Arg.Is<IReadOnlyCollection<object?>>(values => values.Count == 0))
            .Returns(new RenderOutput("FIRST", new object?[] { 10 }));

        secondClause.Render(
                _quoter,
                _placeholders,
                Arg.Is<IReadOnlyCollection<object?>>(values => values.SequenceEqual(new object?[] { 10 })))
            .Returns(new RenderOutput("SECOND", new object?[] { 10, 20 }));

        _binder.Bind(10).Returns("bound-10");
        _binder.Bind(20).Returns("bound-20");
        query.AddClause(secondClause);
        query.AddClause(firstClause);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Sql.Should().Be("SELECT * FROM \"student\" FIRST SECOND");
        result.Bindings.Should().Equal("bound-10", "bound-20");
    }

    [Fact]
    public void Compile_WhenClauseRendersBlankSql_ShouldExcludeClauseFromSql()
    {
        // Arrange
        var query = new Query().From("student");
        var blankClause = CreateClause(20, "   ");
        _quoter.Quote("student").Returns("\"student\"");
        query.AddClause(blankClause);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Sql.Should().Be("SELECT * FROM \"student\"");
    }

    private IQueryClause CreateClause(int order, string sql)
    {
        var clause = Substitute.For<IQueryClause>();
        clause.Order.Returns(order);
        clause.Render(
                _quoter,
                _placeholders,
                Arg.Any<IReadOnlyCollection<object?>>())
            .Returns(call => new RenderOutput(
                sql,
                call.ArgAt<IReadOnlyCollection<object?>>(2)));

        return clause;
    }
}
