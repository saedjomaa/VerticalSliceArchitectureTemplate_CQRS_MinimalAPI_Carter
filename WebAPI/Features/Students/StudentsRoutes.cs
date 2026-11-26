namespace WebAPI.Features.Students;

/// <summary>
/// What every Students endpoint shares: the route prefix, the OpenAPI tag and the route names
/// used by one slice to link to another (e.g. CreateStudent's Location points to GetStudent).
/// </summary>
internal static class StudentsRoutes
{
    public const string GetStudent = "GetStudent";

    public static RouteGroupBuilder MapStudentsGroup(this IEndpointRouteBuilder app) =>
        app.MapGroup("api/students").WithTags("Students");
}
