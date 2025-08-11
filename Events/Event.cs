namespace event_sourcing.Events;

public abstract record Event
{
    public abstract Guid StreamId { get; }
    public abstract int Version { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
}

