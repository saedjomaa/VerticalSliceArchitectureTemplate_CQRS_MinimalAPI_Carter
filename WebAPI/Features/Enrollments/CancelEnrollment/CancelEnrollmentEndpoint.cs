using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.Enrollments.CancelEnrollment;

public sealed class CancelEnrollmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapEnrollmentsGroup()
            .MapDelete(EnrollmentsRoutes.Enrollment, HandleAsync)
            .WithSummary("Cancel a student's enrollment and release the seat")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> HandleAsync(
        Guid studentId,
        Guid courseId,
        ICommandHandler<CancelEnrollmentCommand, Result> handler,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new CancelEnrollmentCommand(studentId, courseId), cancellationToken);
        return result.IsSuccess ? TypedResults.NoContent() : result.ToProblemDetails(context);
    }
}
