using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.Enrollments.GetEnrollment;

public sealed class GetEnrollmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapEnrollmentsGroup()
            .MapGet(EnrollmentsRoutes.Enrollment, HandleAsync)
            .WithSummary("Get a student's enrollment in a course")
            .Produces<EnrollmentResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid studentId,
        Guid courseId,
        IQueryHandler<GetEnrollmentQuery, Result<EnrollmentResponse>> handler,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetEnrollmentQuery(studentId, courseId), cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToProblemDetails(context);
    }
}
