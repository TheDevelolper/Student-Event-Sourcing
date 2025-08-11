namespace event_sourcing.Events.v1;

public abstract record StudentEvent : Event
{
    public abstract Guid StudentId { get; init; }
    public override Guid StreamId => StudentId; // the stream ID for the student is the studentID

    public override int Version { get; set; } = 1;
}

public record StudentCreated(Guid StudentId , string FullName, string Email) : StudentEvent;
public record StudentUpdated(Guid StudentId , string FullName, string Email) : StudentEvent;
public record StudentDeleted(Guid StudentId) : StudentEvent;
public record StudentEnrolled(Guid StudentId, string CourseName) : StudentEvent;
public record StudentUnEnrolled(Guid StudentId, string CourseName) : StudentEvent;