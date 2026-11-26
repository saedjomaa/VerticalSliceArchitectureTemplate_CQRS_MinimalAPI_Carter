using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Pagination;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.JsonPlaceholder.ListUsers;

public sealed class ListUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapJsonPlaceholderGroup()
            .MapGet("/users", HandleAsync)
            .WithSummary("List users from JSONPlaceholder")
            .Produces<PagedResult<JsonPlaceholderUserResponse>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status502BadGateway)
            .ProducesProblem(StatusCodes.Status504GatewayTimeout);

    private static async Task<IResult> HandleAsync(
        IQueryHandler<ListUsersQuery, Result<PagedResult<JsonPlaceholderUserResponse>>> handler,
        HttpContext context,
        CancellationToken cancellationToken,
        int page = Pagination.DefaultPage,
        int pageSize = Pagination.DefaultPageSize)
    {
        var result = await handler.HandleAsync(new ListUsersQuery(page, pageSize), cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToProblemDetails(context);
    }
}
