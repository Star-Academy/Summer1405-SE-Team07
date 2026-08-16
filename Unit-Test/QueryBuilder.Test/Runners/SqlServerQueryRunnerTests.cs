

using System.Data.Common;
using System.Data;
using FluentAssertions;
using NSubstitute;
using QueryLib.Compilers;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.QueryRunners;

namespace QueryBuilder.Test.Runners;


public class SqlServerQueryRunnerTests
{
    private readonly IQueryRunner _sut;

    public SqlServerQueryRunnerTests()
    {
        _sut = new SqlServerQueryRunner();
    }

    [Fact]
    public async Task RunAsync_ShouldThrowInvalidOperationException_WhenConnectionIsClosed()
    {
        // Arrange
        var query = new CompiledQuery("SELECT * FROM TestTable", new List<object?>());
        var connection = Substitute.For<DbConnection>();
        connection.State.Returns(System.Data.ConnectionState.Closed);

        // Act
        var act = () => _sut.RunAsync(query, connection);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("The provided connection is not open.*");
        connection.DidNotReceive().CreateCommand();
    }

    [Fact]
    public async Task RunAsync_ShouldExecuteQueryAndMapResult_WhenConnectionIsOpen()
    {
        // Arrange
        var query = new CompiledQuery(
            "SELECT id, name FROM TestTable WHERE id = @p0 AND name = @p1",
            new List<object?> { 42, null });

        var connection = Substitute.For<DbConnection>();
        var command = Substitute.For<DbCommand>();
        var parameters = Substitute.For<DbParameterCollection>();
        var idParameter = Substitute.For<DbParameter>();
        var nameParameter = Substitute.For<DbParameter>();
        var transaction = Substitute.For<DbTransaction>();
        var reader = Substitute.For<DbDataReader>();

        connection.State.Returns(ConnectionState.Open);
        connection.CreateCommand().Returns(command);
        command.Parameters.Returns(parameters);
        command.CreateParameter().Returns(idParameter, nameParameter);
        command.ExecuteReaderAsync().Returns(reader);

        reader.FieldCount.Returns(2);
        reader.GetName(0).Returns("id");
        reader.GetName(1).Returns("name");
        reader.ReadAsync().Returns(true, false);
        reader.GetValue(0).Returns(42);
        reader.GetValue(1).Returns(DBNull.Value);

        // Act
        var result = await _sut.RunAsync(query, connection, transaction);

        // Assert
        command.CommandText.Should().Be(query.Sql);
        command.Transaction.Should().BeSameAs(transaction);
        idParameter.ParameterName.Should().Be("@p0");
        idParameter.Value.Should().Be(42);
        nameParameter.ParameterName.Should().Be("@p1");
        nameParameter.Value.Should().Be(DBNull.Value);
        parameters.Received(1).Add(idParameter);
        parameters.Received(1).Add(nameParameter);

        result.ColumnNames.Should().Equal("id", "name");
        var row = result.Rows.Should().ContainSingle().Which;
        row["id"].Should().Be(42);
        row["name"].Should().BeNull();
    }
}
