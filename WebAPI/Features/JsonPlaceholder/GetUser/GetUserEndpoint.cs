using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.JsonPlaceholder.GetUser;

public sealed class GetUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapJsonPlaceholderGroup()
            .MapGet("/users/{id:int}", HandleAsync)
            .WithSummary("Get a user from JSONPlaceholder")
            .Produces<JsonPlaceholderUserResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status502BadGateway)
            .ProducesProblem(StatusCodes.Status504GatewayTimeout);

    private static async Task<IResult> HandleAsync(
        int id,
        IQueryHandler<GetUserQuery, Result<JsonPlaceholderUserResponse>> handler,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetUserQuery(id), cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToProblemDetails(context);
    }
}
