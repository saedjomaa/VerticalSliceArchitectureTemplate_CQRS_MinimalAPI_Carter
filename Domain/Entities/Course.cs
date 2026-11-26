namespace Domain.Entities;

public sealed class Course
{
    public const int MaximumTitleLength = 120;
    public const int DefaultCapacity = 30;

    private Course() { }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public int Capacity { get; private set; }
    public int EnrolledCount { get; private set; }
    public Guid RowVersion { get; private set; }

    public static Course Create(string title, int capacity = DefaultCapacity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(title.Length, MaximumTitleLength);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        return new Course
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Capacity = capacity,
            RowVersion = Guid.NewGuid()
        };
    }

    public Enrollment? TryEnroll(Guid studentId, DateTimeOffset enrolledAt)
    {
        if (EnrolledCount >= Capacity)
        {
            return null;
        }

        var enrollment = Enrollment.Create(studentId, Id, enrolledAt);
        EnrolledCount++;
        RowVersion = Guid.NewGuid();
        return enrollment;
    }

    public void CancelEnrollment(Enrollment enrollment)
    {
        if (enrollment.CourseId != Id || EnrolledCount <= 0)
        {
            throw new InvalidOperationException("The enrollment cannot be canceled for this course.");
        }

        EnrolledCount--;
        RowVersion = Guid.NewGuid();
    }
}
