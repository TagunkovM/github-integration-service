namespace GitHubIntegration.Application.Exceptions;

public sealed class ExternalServiceException(
    string message,
    int? statusCode = null,
    DateTimeOffset? rateLimitResetAt = null,
    Exception? innerException = null) : Exception(message, innerException)
{
    public int? StatusCode { get; } = statusCode;
    public DateTimeOffset? RateLimitResetAt { get; } = rateLimitResetAt;
}
