namespace WebAPI.Features.JsonPlaceholder;

/// <summary>
/// What every JsonPlaceholder endpoint shares: one route prefix and one OpenAPI tag per external service.
/// The database is not used by this module.
/// </summary>
internal static class JsonPlaceholderRoutes
{
    public static RouteGroupBuilder MapJsonPlaceholderGroup(this IEndpointRouteBuilder app) =>
        app.MapGroup("api/jsonplaceholder").WithTags("JsonPlaceholder");
}
