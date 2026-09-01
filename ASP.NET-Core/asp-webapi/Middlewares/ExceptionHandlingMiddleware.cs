using asp_webapi.Exceptions;

namespace asp_webapi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnsupportedDatabaseTypeException ex)
        {
            await WriteError(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            await WriteError(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");
            await WriteError(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task WriteError(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new { Message = message });
    }
}