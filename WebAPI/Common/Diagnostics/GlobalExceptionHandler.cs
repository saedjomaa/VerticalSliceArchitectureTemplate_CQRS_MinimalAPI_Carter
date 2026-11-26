using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace WebAPI.Common.Diagnostics;

/// <summary>
/// Handles two kinds of exceptions with a uniform ProblemDetails:
/// - <see cref="BadHttpRequestException"/>: Minimal API could not bind the request (invalid JSON, missing body,
///   wrong value type). <c>RouteHandlerOptions.ThrowOnBadRequest</c> is enabled in <c>Program.cs</c> so these
///   always reach this handler instead of returning an empty 400. Returned as 400 with code <c>Request.Invalid</c>.
/// - anything else: unexpected error, returned as 500.
/// Exception messages are logged, never returned to the client.
/// </summary>
internal sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions> jsonOptions) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int statusCode;
        Microsoft.AspNetCore.Mvc.ProblemDetails problem;

        if (exception is BadHttpRequestException badRequest)
        {
            logger.LogWarning(exception, "The request could not be bound");
            statusCode = badRequest.StatusCode;
            problem = ProblemDetailsBuilder.Create(httpContext, statusCode, "The request is invalid or malformed.");
            problem.Extensions["code"] = "Request.Invalid";
        }
        else
        {
            logger.LogError(exception, "An unexpected exception occurred while handling the request");
            statusCode = StatusCodes.Status500InternalServerError;
            problem = ProblemDetailsBuilder.Create(httpContext, statusCode, "An unexpected error occurred.");
        }

        var options = jsonOptions.Value.SerializerOptions;
        logger.Log(
            statusCode >= StatusCodes.Status500InternalServerError ? LogLevel.Error : LogLevel.Warning,
            "Responded {StatusCode} with {ProblemDetails}",
            statusCode,
            JsonSerializer.Serialize(problem, problem.GetType(), options));

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            problem, problem.GetType(), options, "application/problem+json", cancellationToken);
        return true;
    }
}
