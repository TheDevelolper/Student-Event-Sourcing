using System.Text.Json;
using event_sourcing.DomainModels;
using event_sourcing.DomainModels.Extensions;
using event_sourcing.Events;
using event_sourcing.Events.v1;
using event_sourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace event_sourcing.Stores.storedEvents.v1;

public class StudentEventStore
{
    private readonly EventStoreDbContext _context;
    private Dictionary<Guid, SortedList<DateTime, StudentEvent>> _studentEvents = new();

    public StudentEventStore(EventStoreDbContext context)
    {
        _context = context;
        var studentEvents = LoadEventsAsync().GetAwaiter().GetResult();

        foreach (var studentEvent in studentEvents)
        {
            if (_studentEvents.TryGetValue(studentEvent.StreamId, out var currentEvents))
            {
                currentEvents.Add(studentEvent.CreatedAtUtc, studentEvent);
            }
            else
            {
                var events = new SortedList<DateTime, StudentEvent>
                {
                    { studentEvent.CreatedAtUtc, studentEvent }
                };
                _studentEvents.Add(studentEvent.StreamId, events);
            }
        }
    }

    public async Task AppendAsync(StudentEvent @event)
    {
        var stream = _studentEvents!.GetValueOrDefault(@event.StreamId, null);

        if (stream is null)
        {
            stream = new SortedList<DateTime, StudentEvent>();
            _studentEvents[@event.StreamId] = stream;
        }

        @event.CreatedAtUtc = DateTime.UtcNow;

        await SaveEventAsync(@event);

        stream.Add(@event.CreatedAtUtc, @event);
    }

    public Student? GetStudent(Guid studentId)
    {
        if (_studentEvents.ContainsKey(studentId) == false) return null;

        var student = new Student();
        var studentEvents = _studentEvents[studentId];
        foreach (var studentEvent in studentEvents)
        {
            student.Apply(studentEvent.Value);
        }

        return student;
    }
    
    async Task SaveEventAsync(StudentEvent @event)
    {
        var storedEvent = new StoredEvent().FromEvent(@event);
        _context.Events.Add(storedEvent);
        await _context.SaveChangesAsync();
    }

    private async Task<List<StudentEvent>> LoadEventsAsync()
    {
        var storedEvents = await _context.Events.OrderBy(e => e.OccurredAt).ToListAsync();

        var events = new List<StudentEvent>();

        foreach (var storedEvent in storedEvents)
        {
            // You can use the Type property to determine which event to deserialize to
            var evt = storedEvent.ToEvent();

            if (evt is StudentEvent studentEvent)
            {
                events.Add(studentEvent);
            }
        }

        return events;
    }


}

public static class StudentEventStoreExtensions
{
    public static StoredEvent FromEvent(this StoredEvent storedEvent, StudentEvent @event)
    {
        var eventTypeName = @event.GetType().Name;
        var serialisedData = eventTypeName switch
        {
            nameof(StudentCreated) => JsonSerializer.Serialize(@event as StudentCreated),
            nameof(StudentUpdated) => JsonSerializer.Serialize(@event as StudentUpdated),
            nameof(StudentDeleted) => JsonSerializer.Serialize(@event as StudentDeleted),
            nameof(StudentEnrolled) => JsonSerializer.Serialize(@event as StudentEnrolled),
            nameof(StudentUnEnrolled) => JsonSerializer.Serialize(@event as StudentUnEnrolled),
            _ => throw new ArgumentOutOfRangeException()
        };
            
        StoredEvent result = new()
        {
            Type = eventTypeName, // "e.g. StudentCreated"
            StreamId = @event.StreamId,
            Version = @event.Version,
            OccurredAt = @event.CreatedAtUtc,
            Data = serialisedData
        };

        return result;
    }

    public static StudentEvent? ToEvent(this StoredEvent storedEvent)
    {
        return storedEvent.Type switch
        {
            nameof(StudentCreated) => JsonSerializer.Deserialize<StudentCreated>(storedEvent.Data),
            nameof(StudentUpdated) => JsonSerializer.Deserialize<StudentUpdated>(storedEvent.Data),
            nameof(StudentDeleted) => JsonSerializer.Deserialize<StudentDeleted>(storedEvent.Data),
            nameof(StudentEnrolled) => JsonSerializer.Deserialize<StudentEnrolled>(storedEvent.Data),
            nameof(StudentUnEnrolled) => JsonSerializer.Deserialize<StudentUnEnrolled>(storedEvent.Data),
            _ => null
        };
    }
}