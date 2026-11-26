using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Pagination;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.Enrollments.ListStudentCourses;

public sealed class ListStudentCoursesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapEnrollmentsGroup()
            .MapGet("/courses", HandleAsync)
            .WithSummary("List courses a student is enrolled in")
            .Produces<PagedResult<StudentCourseResponse>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid studentId,
        IQueryHandler<ListStudentCoursesQuery, Result<PagedResult<StudentCourseResponse>>> handler,
        HttpContext context,
        CancellationToken cancellationToken,
        int page = Pagination.DefaultPage,
        int pageSize = Pagination.DefaultPageSize)
    {
        var result = await handler.HandleAsync(new ListStudentCoursesQuery(studentId, page, pageSize), cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToProblemDetails(context);
    }
}
