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

namespace QueryBuilder.Test;

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
    public void ConstructorShouldThrowExeption_WhenQuoterIsNull()
    {
        // Arrange
        var act = () => new SqlCompiler(null!, _placeholders, _binder);

        // Act & Assert
        
        act.Should().Throw<ArgumentNullException>().WithParameterName("quoter");
    }
    
    [Fact]
    public void ConstructorShouldThrowExeption_WhenPlaceholderIsNull()
    {
        var act = () => new SqlCompiler(_quoter, null!, _binder);
        act.Should().Throw<ArgumentNullException>().WithParameterName("placeholders");
    }

    [Fact]
    public void ConstructorShouldThrowExeption_WhenBinderIsNull()
    {
        var act = () => new SqlCompiler(_quoter, _placeholders, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("binder");
        
    }

    [Fact]
    public void shouldbe()
    {
        var testQuery = new Query().From("Student");
            
        
        _quoter.Quote("Student").Returns("\"Student\"");
        
        var compiledQuery = _sut.Compile(testQuery);
        IReadOnlyList<object?> bindings = Array.Empty<object?>();
        //var expecred_Query = new CompiledQuery("SELECT * FROM \"Student\"" );
        
        compiledQuery.Sql.Should().Be("SELECT * FROM \"Student\"");
    }
    




}