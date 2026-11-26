using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.Students.GetStudent;

public sealed class GetStudentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapStudentsGroup()
            .MapGet("/{id:guid}", HandleAsync)
            .WithName(StudentsRoutes.GetStudent)
            .WithSummary("Get a student by ID")
            .Produces<StudentResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid id,
        IQueryHandler<GetStudentQuery, Result<StudentResponse>> handler,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetStudentQuery(id), cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToProblemDetails(context);
    }
}
