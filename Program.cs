// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using event_sourcing.Events.v1;
using event_sourcing.Infrastructure;
using event_sourcing.Stores.storedEvents.v1;

await using var db = new EventStoreDbContext();
db.Database.EnsureCreated();
var studentId = Guid.Parse("a3655b15-aacc-4c32-a75e-8e48fcbebe9c");

var databaseContext = new StudentEventStore(db);

Console.WriteLine("Press \"w\" to write data to the database");

var keyInfo = Console.ReadKey();
var isWrite  = keyInfo.Key == ConsoleKey.W;

if (isWrite)
{ 
    await databaseContext.AppendAsync(new StudentCreated(studentId, "Sanjay Sokhi","Sanjay.Sokhi@eventsource.com"));
    await databaseContext.AppendAsync(new StudentEnrolled(studentId, "Coding for deveLOLpers")); // enrolls for wrong course 
    await databaseContext.AppendAsync(new StudentEnrolled(studentId, "Event sourcing for mega chad, final boss tech bros")); // enrolls for correct course
    await databaseContext.AppendAsync(new StudentUpdated(studentId, "Sanjay Sokhi", "Sanjay@Sokhi.com")); // changes email
    await databaseContext.AppendAsync(new StudentUnEnrolled(studentId, "Coding for deveLOLpers")); // unenrolls from wrong course
}

var student = databaseContext.GetStudent(studentId);
if (student == null)
{
    Console.WriteLine("Student not found in the database, You must write to the database before you can read. Have you done this?");
    return 1;
}

var studentJson = JsonSerializer.Serialize(student, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(studentJson);
Console.ReadLine();

return 0;