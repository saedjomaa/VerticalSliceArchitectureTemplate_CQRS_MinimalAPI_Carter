using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.Courses.CreateCourse;

public sealed class CreateCourseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapCoursesGroup()
            .MapPost("/", HandleAsync)
            .WithSummary("Create a course")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

    private static async Task<IResult> HandleAsync(
        CreateCourseCommand command,
        ICommandHandler<CreateCourseCommand, Result<Guid>> handler,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.CreatedAtRoute(result.Value, CoursesRoutes.GetCourse, new { id = result.Value })
            : result.ToProblemDetails(context);
    }
}
