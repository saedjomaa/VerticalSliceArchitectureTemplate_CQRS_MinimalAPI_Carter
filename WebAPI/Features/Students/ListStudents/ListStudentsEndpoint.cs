using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Pagination;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.Students.ListStudents;

public sealed class ListStudentsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapStudentsGroup()
            .MapGet("/", HandleAsync)
            .WithSummary("List students")
            .Produces<PagedResult<StudentResponse>>()
            .ProducesValidationProblem();

    private static async Task<IResult> HandleAsync(
        IQueryHandler<ListStudentsQuery, Result<PagedResult<StudentResponse>>> handler,
        HttpContext context,
        CancellationToken cancellationToken,
        int page = Pagination.DefaultPage,
        int pageSize = Pagination.DefaultPageSize)
    {
        var result = await handler.HandleAsync(new ListStudentsQuery(page, pageSize), cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToProblemDetails(context);
    }
}
