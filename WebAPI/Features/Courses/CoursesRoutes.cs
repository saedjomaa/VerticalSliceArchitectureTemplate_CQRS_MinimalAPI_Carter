namespace WebAPI.Features.Courses;

/// <summary>
/// What every Courses endpoint shares: the route prefix, the OpenAPI tag and the route names
/// used by one slice to link to another (e.g. CreateCourse's Location points to GetCourse).
/// </summary>
internal static class CoursesRoutes
{
    public const string GetCourse = "GetCourse";

    public static RouteGroupBuilder MapCoursesGroup(this IEndpointRouteBuilder app) =>
        app.MapGroup("api/courses").WithTags("Courses");
}
