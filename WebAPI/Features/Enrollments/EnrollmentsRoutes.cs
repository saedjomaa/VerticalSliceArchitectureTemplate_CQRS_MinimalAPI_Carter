namespace WebAPI.Features.Enrollments;

/// <summary>
/// What every Enrollments endpoint shares: enrollments are nested under a student
/// (<c>api/students/{studentId}</c>) and documented under the Enrollments tag.
/// </summary>
internal static class EnrollmentsRoutes
{
    /// <summary>One enrollment of the student, relative to the group.</summary>
    public const string Enrollment = "/enrollments/{courseId:guid}";

    public static RouteGroupBuilder MapEnrollmentsGroup(this IEndpointRouteBuilder app) =>
        app.MapGroup("api/students/{studentId:guid}").WithTags("Enrollments");
}
