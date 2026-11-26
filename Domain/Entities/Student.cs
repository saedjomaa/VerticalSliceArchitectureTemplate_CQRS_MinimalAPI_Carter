namespace Domain.Entities;

public sealed class Student
{
    public const int MaximumFullNameLength = 120;

    private Student() { }

    public Guid Id { get; private set; }
    public string FullName { get; private set; } = null!;

    public static Student Create(string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(fullName.Length, MaximumFullNameLength);

        return new Student
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim()
        };
    }
}
