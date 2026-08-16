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
    public void Constructor_ShouldThrowArgumentNullException_WhenQuoterIsNull()
    {
        // Arrange
        var expected = "quoter";

        // Act
        var act = () => new SqlCompiler(null!, _placeholders, _binder);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName(expected);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenPlaceholderFactoryIsNull()
    {
        // Arrange
        var expected = "placeholders";
        
        // Act
        var act = () => new SqlCompiler(_quoter, null!, _binder);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName(expected);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenBinderIsNull()
    {
        // Arrange
        var expected = "binder";
        
        // Act
        var act = () => new SqlCompiler(_quoter, _placeholders, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName(expected);
    }

    [Fact]
    public void Compile_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Arrange
        var expected = "query";
        
        // Act
        var act = () => _sut.Compile(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName(expected);
    }

    [Fact]
    public void Compile_ShouldThrowInvalidOperationException_WhenFromClauseIsMissing()
    {
        // Arrange
        var query = new Query();
        var expected = "From(...) must be called before compiling the query.";
        // Act
        var act = () => _sut.Compile(query);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage(expected);
    }

    [Fact]
    public void Compile_ShouldUseWildcardAndReturnNoBindings_WhenNoColumnsAreSelected()
    {
        // Arrange
        var query = new Query().From("student");
        _quoter.Quote("student").Returns("\"student\"");
        var expected = new CompiledQuery("SELECT * FROM \"student\"" , new object?[] { });

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().BeEquivalentTo(expected);
        _binder.DidNotReceive().Bind(Arg.Any<object?>());
    }

    [Fact]
    public void Compile_ShouldCompileSqlAndBindValues_WhenColumnsAndConditionsExist()
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
        var expected = new CompiledQuery("SELECT \"id\", \"name\" FROM \"student\" " +
                                         "WHERE \"id\" = @p1 AND \"status\" = @p2" ,
            new object?[] { "bound-id", "bound-status"});
        
        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Compile_ShouldRenderByClauseOrder_WhenClausesAreAddedOutOfOrder()
    {
        // Arrange
        var query = new Query().From("student");
        var orderByClause = CreateClause(30, "ORDER BY name");
        var whereClause = CreateClause(20, "WHERE age > @p1");
        _quoter.Quote("student").Returns("\"student\"");
        var expected = new CompiledQuery("SELECT * FROM \"student\" WHERE age > @p1 ORDER BY name" ,
            new object?[] { });

        query.AddClause(orderByClause);
        query.AddClause(whereClause);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Compile_ShouldPassBindingsBetweenClauses_WhenMultipleClausesProduceBindings()
    {
        // Arrange
        var query = new Query().From("student");
        var firstClause = Substitute.For<IQueryClause>();
        var secondClause = Substitute.For<IQueryClause>();
        firstClause.Order.Returns(20);
        secondClause.Order.Returns(30);
        _quoter.Quote("student").Returns("\"student\"");
        var expected = new CompiledQuery("SELECT * FROM \"student\" FIRST SECOND" ,
            new object?[] {"bound-10", "bound-20" });
        
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
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Compile_ShouldExcludeClauseFromSql_WhenClauseRendersBlankSql()
    {
        // Arrange
        var query = new Query().From("student");
        var blankClause = CreateClause(20, "   ");
        _quoter.Quote("student").Returns("\"student\"");
        query.AddClause(blankClause);
        var expected = "SELECT * FROM \"student\"";

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Sql.Should().Be(expected);
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
