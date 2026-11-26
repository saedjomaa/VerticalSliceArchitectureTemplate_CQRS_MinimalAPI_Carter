using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Common.Diagnostics;

namespace WebAPI.Common.Results;

internal static class ResultExtensions
{
    /// <summary>Converts a failed <see cref="Result"/> to a Minimal API <see cref="ProblemHttpResult"/>.</summary>
    public static ProblemHttpResult ToProblemDetails(this IResult result, HttpContext context)
    {
        var error = result.Error ?? throw new InvalidOperationException("A successful result has no error.");
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.BadGateway => StatusCodes.Status502BadGateway,
            ErrorType.GatewayTimeout => StatusCodes.Status504GatewayTimeout,
            _ => throw new ArgumentOutOfRangeException(nameof(result), error.Type, "Unknown error type.")
        };

        var problem = ProblemDetailsBuilder.Create(
            context,
            status,
            error.Description,
            error.ValidationErrors?.ToDictionary(pair => pair.Key, pair => pair.Value));
        problem.Extensions["code"] = error.Code;

        return TypedResults.Problem(problem);
    }
}
