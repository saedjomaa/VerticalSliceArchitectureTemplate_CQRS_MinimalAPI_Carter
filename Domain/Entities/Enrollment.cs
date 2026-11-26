namespace Domain.Entities;

public sealed class Enrollment
{
    private Enrollment() { }

    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public DateTimeOffset EnrolledAt { get; private set; }

    internal static Enrollment Create(Guid studentId, Guid courseId, DateTimeOffset enrolledAt)
    {
        return new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = enrolledAt
        };
    }
}
