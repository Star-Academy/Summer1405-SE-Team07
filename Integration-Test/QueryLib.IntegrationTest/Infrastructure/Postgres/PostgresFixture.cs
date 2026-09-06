using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace QueryLib.IntegrationTest.Infrastructure.Postgres;

public sealed class PostgresFixture : IAsyncLifetime
{
    private PostgreSqlContainer PostgresContainer { get; set; } = null!;
    public string ConnectionString => PostgresContainer.GetConnectionString();
    public ServiceProvider? ServiceProvider { get; private set; }

    public ICompiler Compiler => ServiceProvider!.GetRequiredService<ICompiler>();

    public async Task InitializeAsync()
    {
        PostgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("postgresDB")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await PostgresContainer.StartAsync();
        await SeedDatabaseAsync();


        var services = new ServiceCollection();
        services.AddQueryLibServices();
        ServiceProvider = services.BuildServiceProvider();
    }

    private async Task SeedDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = """
            DROP TABLE IF EXISTS "Student";

            CREATE TABLE "Student"
            (
                "StudentNumber" VARCHAR(8) NOT NULL,
                "Grade" FLOAT,
                "FirstName" VARCHAR(20) NOT NULL,
                "LastName" VARCHAR(20) NOT NULL,
                "IsMale" BOOLEAN NOT NULL,
                "DateOfBirth" TIMESTAMP NOT NULL,
                "LeftUnitsCount" INT NOT NULL
            );

            INSERT INTO "Student"
            VALUES
            ('98100200', 13.234, 'John', 'Smith', TRUE, '2001-01-22', 92),
            ('98100201', 17.850, 'Michael', 'Johnson', TRUE, '2000-05-14', 45),
            ('98100202', 19.120, 'Emily', 'Williams', FALSE, '2002-03-09', 30),
            ('98100203', 14.560, 'David', 'Brown', TRUE, '1999-11-25', 78),
            ('98100204', 16.750, 'Sarah', 'Jones', FALSE, '2001-07-18', 54),
            ('98100205', 12.980, 'James', 'Garcia', TRUE, '2000-09-30', 101),
            ('98100206', 18.430, 'Olivia', 'Miller', FALSE, '2002-12-05', 22),
            ('98100207', 15.670, 'Robert', 'Davis', TRUE, '2001-04-11', 67),
            ('98100208', 11.250, 'Sophia', 'Martinez', FALSE, '1998-08-20', 120),
            ('98100209', 20.000, 'William', 'Wilson', TRUE, '2000-02-15', 10),
            ('98100210', 13.900, 'Emma', 'Anderson', FALSE, '2001-10-03', 88),
            ('98100211', 16.340, 'Daniel', 'Taylor', TRUE, '1999-06-27', 73),
            ('98100212', 14.780, 'Ava', 'Thomas', FALSE, '2002-01-19', 60),
            ('98100213', 17.560, 'Christopher', 'Moore', TRUE, '2000-12-12', 35),
            ('98100214', 12.450, 'Mia', 'Jackson', FALSE, '2001-03-29', 95),
            ('98100215', 18.900, 'Matthew', 'Martin', TRUE, '1999-09-08', 40),
            ('98100216', 15.230, 'Charlotte', 'Lee', FALSE, '2002-06-16', 70),
            ('98100217', 19.450, 'Joseph', 'Perez', TRUE, '2000-07-23', 18),
            ('98100218', 13.670, 'Amelia', 'White', FALSE, '2001-11-01', 82),
            ('98100219', 16.890, 'Andrew', 'Harris', TRUE, '1998-04-07', 110);
            """;

        await command.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        if (ServiceProvider is not null)
        {
            await ServiceProvider.DisposeAsync();
        }

        await PostgresContainer.DisposeAsync();
    }
}


