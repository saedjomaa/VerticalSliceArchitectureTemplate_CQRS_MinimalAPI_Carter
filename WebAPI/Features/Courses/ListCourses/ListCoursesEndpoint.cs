using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Pagination;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.Courses.ListCourses;

public sealed class ListCoursesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapCoursesGroup()
            .MapGet("/", HandleAsync)
            .WithSummary("List courses")
            .Produces<PagedResult<CourseResponse>>()
            .ProducesValidationProblem();

    private static async Task<IResult> HandleAsync(
        IQueryHandler<ListCoursesQuery, Result<PagedResult<CourseResponse>>> handler,
        HttpContext context,
        CancellationToken cancellationToken,
        int page = Pagination.DefaultPage,
        int pageSize = Pagination.DefaultPageSize)
    {
        var result = await handler.HandleAsync(new ListCoursesQuery(page, pageSize), cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToProblemDetails(context);
    }
}
