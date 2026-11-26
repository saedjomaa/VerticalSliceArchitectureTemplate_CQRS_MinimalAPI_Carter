using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace WebAPI.Common.Diagnostics;

/// <summary>
/// Builds every ProblemDetails the API returns, complete before it is written, so the logged JSON
/// and the JSON sent to the client are the same. Minimal API has no MVC ProblemDetailsFactory;
/// this fills the same fields it did: type, title, status, detail, traceId and correlationId.
/// </summary>
internal static class ProblemDetailsBuilder
{
    private static readonly Dictionary<int, string> Types = new()
    {
        [StatusCodes.Status400BadRequest] = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        [StatusCodes.Status404NotFound] = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        [StatusCodes.Status409Conflict] = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        [StatusCodes.Status500InternalServerError] = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
        [StatusCodes.Status502BadGateway] = "https://tools.ietf.org/html/rfc9110#section-15.6.3",
        [StatusCodes.Status504GatewayTimeout] = "https://tools.ietf.org/html/rfc9110#section-15.6.5"
    };

    /// <param name="validationErrors">When set, an <see cref="HttpValidationProblemDetails"/> with an <c>errors</c> field is built.</param>
    public static ProblemDetails Create(
        HttpContext context, int status, string detail, IDictionary<string, string[]>? validationErrors = null)
    {
        ProblemDetails problem = validationErrors is null
            ? new ProblemDetails()
            : new HttpValidationProblemDetails(validationErrors);

        problem.Status = status;
        problem.Detail = detail;
        problem.Type ??= Types.GetValueOrDefault(status);
        problem.Title ??= ReasonPhrases.GetReasonPhrase(status);
        problem.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
        problem.Extensions["correlationId"] = context.TraceIdentifier;
        return problem;
    }
}
