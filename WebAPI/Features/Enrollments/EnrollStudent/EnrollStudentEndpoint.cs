using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.Enrollments.EnrollStudent;

public sealed class EnrollStudentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapEnrollmentsGroup()
            .MapPost("/enrollments", HandleAsync)
            .WithSummary("Create a course enrollment for a student")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> HandleAsync(
        Guid studentId,
        EnrollStudentRequest request,
        ICommandHandler<EnrollStudentCommand, Result> handler,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new EnrollStudentCommand(studentId, request.CourseId), cancellationToken);
        return result.IsSuccess ? TypedResults.NoContent() : result.ToProblemDetails(context);
    }
}
