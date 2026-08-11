using QueryLib.Compilers;
using QueryLib.Dialects.Abstractions;
using QueryLib.Compilers.Abstractions;
using System;
using System.Collections.Generic;
using System;
using NSubstitute;
using Xunit;
using FluentAssertions;
using QueryLib;
using QueryLib.Clauses.Abstractions;

namespace QueryBuilder.Test.Compilers;

public class SqlCompilerTests
{
    private readonly IIdentifierQuoter _quoter;
    private readonly IParameterPlaceholderFactory _placeholders;
    private readonly IValueBinder _binder;
    private readonly SqlCompiler _sut;
    
    public SqlCompilerTests()
    {
        _quoter = Substitute.For<IIdentifierQuoter>();
        _placeholders = Substitute.For<IParameterPlaceholderFactory>();
        _binder = Substitute.For<IValueBinder>();
        _sut = new SqlCompiler(_quoter, _placeholders, _binder);
        
    }
    
    [Fact]
    public void Constructor_ShouldThrowExeption_WhenQuoterIsNull()
    {
        // Arrange
        var act = () => new SqlCompiler(null!, _placeholders, _binder);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("quoter");
    }
    
    [Fact]
    public void Constructor_ShouldThrowExeption_WhenPlaceholderIsNull()
    {
        // Arrange
        var act = () => new SqlCompiler(_quoter, null!, _binder);
        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("placeholders");
    }

    [Fact]
    public void Constructor_ShouldThrowExeption_WhenBinderIsNull()
    {
        // Arrange
        var act = () => new SqlCompiler(_quoter, _placeholders, null!);
        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("binder");
        
    }

    
    [Fact]
    public void Compile_ShouldUseStar_WhenQueryHasNoColumns()
    {
        // Arrange
        var query = new Query()
            .From("student");

        _quoter
            .Quote("student")
            .Returns("\"student\"");

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Sql.Should().Be("SELECT * FROM \"student\"");
        
        // _quoter
        //     .Received(1)
        //     .Quote("name");

    }
  
    
    [Fact]
    public void Compile_ShouldAddClauseWithBindingsSql_WhenClauseRendersSql()
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("id", "name");

        var clause = Substitute.For<IQueryClause>();

        clause.Order.Returns(20);

        clause.Render(
                _quoter,
                _placeholders,
                Arg.Any<IReadOnlyCollection<object?>>())
            .Returns(
                new RenderOutput(
                    "WHERE \"id\" = @p1",null
                    ));

        _quoter.Quote("student").Returns("\"student\"");
        _quoter.Quote("id").Returns("\"id\"");
        _quoter.Quote("name").Returns("\"name\"");

        _binder.Bind(10).Returns(10);

        query.AddClause(clause);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Sql.Should().Be("SELECT \"id\", \"name\" FROM \"student\" WHERE \"id\" = @p1");

        result.Bindings
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(10);
    }
    
    
    
    [Fact]
    public void Compile_ShouldRenderClausesInOrder()
    {
        // Arrange
        var query = new Query()
            .From("student");

        var firstAddedClause = Substitute.For<IQueryClause>();
        var secondAddedClause = Substitute.For<IQueryClause>();

        firstAddedClause.Order.Returns(20);
        secondAddedClause.Order.Returns(10);

        firstAddedClause
            .Render(
                _quoter,
                _placeholders,
                Arg.Any<IReadOnlyCollection<object?>>())
            .Returns(
                new RenderOutput(
                    "WHERE age > @p1",
                    new object?[] { 18 }));

        secondAddedClause
            .Render(
                _quoter,
                _placeholders,
                Arg.Any<IReadOnlyCollection<object?>>())
            .Returns(
                new RenderOutput(
                    "ORDER BY name",
                    Array.Empty<object?>()));

        _quoter
            .Quote("student")
            .Returns("\"student\"");

        _binder
            .Bind(18)
            .Returns(18);

        // Intentionally add Order 20 first
        query.AddClause(firstAddedClause);
        query.AddClause(secondAddedClause);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Sql.Should()
            .Be("SELECT * FROM \"student\" ORDER BY name WHERE age > @p1");
    }
    
    
    [Fact]
    public void Compile_ShouldCompileWhereClause()
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("id", "name")
            .Where("id", 10);

        _quoter
            .Quote("student")
            .Returns("\"student\"");

        _quoter
            .Quote("id")
            .Returns("\"id\"");

        _placeholders
            .MakePlaceholder(1)
            .Returns("@p1");

        _binder
            .Bind(10)
            .Returns(10);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.Sql.Should()
            .Be("SELECT \"id\", \"name\" FROM \"student\" WHERE \"id\" = @p1");

        result.Bindings
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(10);

        _quoter
            .Received(1)
            .Quote("student");

        _quoter
            .Received(1)
            .Quote("id");

        _placeholders
            .Received(1)
            .MakePlaceholder(1);

        _binder
            .Received(1)
            .Bind(10);
    }
}