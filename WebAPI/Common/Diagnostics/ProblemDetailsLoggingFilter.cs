using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace WebAPI.Common.Diagnostics;

/// <summary>
/// Minimal API endpoint filter (MVC result filters do not run for minimal endpoints), applied to every
/// Carter endpoint in <c>Program.cs</c>. Logs the exact ProblemDetails JSON returned to the client whenever
/// an endpoint fails. Successful responses are not logged.
/// Unexpected exceptions (500) and binding failures are logged by <see cref="GlobalExceptionHandler"/>.
/// </summary>
internal sealed class ProblemDetailsLoggingFilter(
    ILogger<ProblemDetailsLoggingFilter> logger,
    IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions> jsonOptions) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);

        if (result is ProblemHttpResult problemResult)
        {
            var problem = problemResult.ProblemDetails;
            var statusCode = problemResult.StatusCode;
            var json = JsonSerializer.Serialize(problem, problem.GetType(), jsonOptions.Value.SerializerOptions);

            logger.Log(
                statusCode >= StatusCodes.Status500InternalServerError ? LogLevel.Error : LogLevel.Warning,
                "Responded {StatusCode} with {ProblemDetails}",
                statusCode,
                json);
        }

        return result;
    }
}
