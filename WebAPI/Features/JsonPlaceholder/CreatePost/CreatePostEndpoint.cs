using Carter;
using WebAPI.Common.Cqrs;
using WebAPI.Common.Results;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace WebAPI.Features.JsonPlaceholder.CreatePost;

public sealed class CreatePostEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapJsonPlaceholderGroup()
            .MapPost("/posts", HandleAsync)
            .WithSummary("Create a post in JSONPlaceholder (fake: JSONPlaceholder does not persist it)")
            .Produces<JsonPlaceholderPostResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status502BadGateway)
            .ProducesProblem(StatusCodes.Status504GatewayTimeout);

    private static async Task<IResult> HandleAsync(
        CreatePostCommand command,
        ICommandHandler<CreatePostCommand, Result<JsonPlaceholderPostResponse>> handler,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        // There is no endpoint to read a single post, so the 201 has no Location header.
        return result.IsSuccess
            ? TypedResults.Created((string?)null, result.Value)
            : result.ToProblemDetails(context);
    }
}
