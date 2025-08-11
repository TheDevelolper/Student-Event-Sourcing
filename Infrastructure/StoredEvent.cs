namespace event_sourcing.Infrastructure;

public class StoredEvent
{
    public long Id { get; set; }
    public string Type { get; set; } = default!;
    public Guid StreamId { get; set; }
    public int Version { get; set; } // optional for concurrency
    public DateTime OccurredAt { get; set; }
    public string Data { get; set; } = default!; // serialized JSON
}