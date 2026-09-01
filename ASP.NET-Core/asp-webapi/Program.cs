using asp_webapi.Services;
using asp_webapi.Middlewares;
using asp_webapi.Services.Abstractions;
using SqlKata.Compilers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKeyedSingleton<IDbConnectionProvider, SqlServerConnectionProvider>("sqlserver");
builder.Services.AddKeyedSingleton<IDbConnectionProvider, PostgresConnectionProvider>("postgres");

builder.Services.AddKeyedSingleton<Compiler, SqlServerCompiler>("sqlserver");
builder.Services.AddKeyedSingleton<Compiler, PostgresCompiler>("postgres");

builder.Services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
builder.Services.AddSingleton<IStudentRepository, StudentRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();