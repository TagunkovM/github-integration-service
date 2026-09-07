using GitHubIntegration.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GitHubIntegration.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            RequestValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            ExternalServiceException external when external.RateLimitResetAt.HasValue => (StatusCodes.Status429TooManyRequests, "GitHub rate limit exceeded"),
            ExternalServiceException external when external.StatusCode == StatusCodes.Status404NotFound => (StatusCodes.Status404NotFound, "GitHub resource not found"),
            ExternalServiceException external when external.StatusCode >= 500 => (StatusCodes.Status503ServiceUnavailable, "GitHub is unavailable"),
            ExternalServiceException => (StatusCodes.Status502BadGateway, "GitHub request failed"),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected server error")
        };

        if (statusCode >= 500)
        {
            logger.LogError(exception, "Unhandled request failure. TraceId: {TraceId}", context.TraceIdentifier);
        }
        else
        {
            logger.LogWarning(exception, "Request failed with status {StatusCode}. TraceId: {TraceId}", statusCode, context.TraceIdentifier);
        }

        if (context.Response.HasStarted)
        {
            throw exception;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;
        if (exception is ExternalServiceException externalServiceException && externalServiceException.RateLimitResetAt.HasValue)
        {
            problem.Extensions["rateLimitResetAt"] = externalServiceException.RateLimitResetAt.Value;
        }

        await context.Response.WriteAsJsonAsync(problem);
    }
}
