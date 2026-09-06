using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace QueryLib.IntegrationTest.Infrastructure.SqlServer;

public sealed class SqlServerFixture : IAsyncLifetime
{
    private MsSqlContainer MsSqlContainer { get; set; } = null!;
    public string ConnectionString => MsSqlContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        MsSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Your_strong_Password123")
            .Build();

        await MsSqlContainer.StartAsync();
        await SeedDatabaseAsync();
    }

    private async Task SeedDatabaseAsync()
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = """
            DROP TABLE IF EXISTS Student;

            CREATE TABLE Student
            (
                StudentNumber VARCHAR(8) NOT NULL,
                Grade FLOAT,
                FirstName VARCHAR(20) NOT NULL,
                LastName VARCHAR(20) NOT NULL,
                IsMale BIT NOT NULL,
                DateOfBirth DATETIME2 NOT NULL,
                LeftUnitsCount INT NOT NULL
            );

            INSERT INTO Student
            (StudentNumber, Grade, FirstName, LastName, IsMale, DateOfBirth, LeftUnitsCount)
            VALUES
            ('98100200', 13.234, 'John', 'Smith', 1, '2001-01-22', 92),
            ('98100201', 17.850, 'Michael', 'Johnson', 1, '2000-05-14', 45),
            ('98100202', 19.120, 'Emily', 'Williams', 0, '2002-03-09', 30),
            ('98100203', 14.560, 'David', 'Brown', 1, '1999-11-25', 78),
            ('98100204', 16.750, 'Sarah', 'Jones', 0, '2001-07-18', 54),
            ('98100205', 12.980, 'James', 'Garcia', 1, '2000-09-30', 101),
            ('98100206', 18.430, 'Olivia', 'Miller', 0, '2002-12-05', 22),
            ('98100207', 15.670, 'Robert', 'Davis', 1, '2001-04-11', 67),
            ('98100208', 11.250, 'Sophia', 'Martinez', 0, '1998-08-20', 120),
            ('98100209', 20.000, 'William', 'Wilson', 1, '2000-02-15', 10),
            ('98100210', 13.900, 'Emma', 'Anderson', 0, '2001-10-03', 88),
            ('98100211', 16.340, 'Daniel', 'Taylor', 1, '1999-06-27', 73),
            ('98100212', 14.780, 'Ava', 'Thomas', 0, '2002-01-19', 60),
            ('98100213', 17.560, 'Christopher', 'Moore', 1, '2000-12-12', 35),
            ('98100214', 12.450, 'Mia', 'Jackson', 0, '2001-03-29', 95),
            ('98100215', 18.900, 'Matthew', 'Martin', 1, '1999-09-08', 40),
            ('98100216', 15.230, 'Charlotte', 'Lee', 0, '2002-06-16', 70),
            ('98100217', 19.450, 'Joseph', 'Perez', 1, '2000-07-23', 18),
            ('98100218', 13.670, 'Amelia', 'White', 0, '2001-11-01', 82),
            ('98100219', 16.890, 'Andrew', 'Harris', 1, '1998-04-07', 110);
            """;

        await command.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        await MsSqlContainer.DisposeAsync();
    }
}
