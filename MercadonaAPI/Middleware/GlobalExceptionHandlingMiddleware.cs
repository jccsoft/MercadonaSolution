using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MercadonaAPI.Middleware;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError("Global exception: {message}", ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            ProblemDetails problem = new()
            {
                Status = context.Response.StatusCode,
                Type = "Server error",
                Title = "Global exception",
                Detail = ex.Message
            };

            string json = JsonSerializer.Serialize(problem);

            await context.Response.WriteAsync(json);

            context.Response.ContentType = "application/json";

        }
    }
}
