namespace event_sourcing.DomainModels;

public class Entity
{
    public Guid Id { get; set; }
}

public class Student : Entity
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> EnrolledCourses { get; set; } = new();
}