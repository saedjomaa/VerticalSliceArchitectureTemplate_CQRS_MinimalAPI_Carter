using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.Students.CreateStudent;

public sealed class CreateStudentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapStudentsGroup()
            .MapPost("/", HandleAsync)
            .WithSummary("Create a student")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

    private static async Task<IResult> HandleAsync(
        CreateStudentCommand command,
        ICommandHandler<CreateStudentCommand, Result<Guid>> handler,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsSuccess
            ? TypedResults.CreatedAtRoute(result.Value, StudentsRoutes.GetStudent, new { id = result.Value })
            : result.ToProblemDetails(context);
    }
}
