using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.Courses.GetCourse;

public sealed class GetCourseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapCoursesGroup()
            .MapGet("/{id:guid}", HandleAsync)
            .WithName(CoursesRoutes.GetCourse)
            .WithSummary("Get a course by ID")
            .Produces<CourseResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid id,
        IQueryHandler<GetCourseQuery, Result<CourseResponse>> handler,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetCourseQuery(id), cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToProblemDetails(context);
    }
}
