using event_sourcing.Events;
using event_sourcing.Events.v1;

namespace event_sourcing.DomainModels.Extensions;

public static class StudentEventSourcingExtensions 
{
    public static void Apply(this Student student, Event @event)
    {
        switch (@event)
        {
            case StudentCreated studentCreated:
                ApplyEvent(student, studentCreated);
                break;
            case StudentUpdated studentUpdated:
                ApplyEvent(student, studentUpdated);
                break;
            case StudentEnrolled studentEnrolled:
                ApplyEvent(student, studentEnrolled);
                break;
            case StudentUnEnrolled studentUnEnrolled:
                ApplyEvent(student, studentUnEnrolled);
                break;
        }
    }
    
    private static void ApplyEvent(Student student, StudentCreated studentCreated)
    {
        student.Id = studentCreated.StudentId;
        student.FullName = studentCreated.FullName;
        student.Email = studentCreated.Email;
    }
    
    private static void ApplyEvent(Student student,StudentUpdated studentUpdated)
    {
        student.FullName = studentUpdated.FullName;
        student.Email = studentUpdated.Email;
    }
    
    private static void ApplyEvent(Student student, StudentEnrolled studentEnrolled)
    {
        student.EnrolledCourses.Add(studentEnrolled.CourseName);
    }  
    
    private static void ApplyEvent(Student student, StudentUnEnrolled studentUnEnrolled)
    {
        if (!student.EnrolledCourses.Contains(studentUnEnrolled.CourseName)) return;
        student.EnrolledCourses.Remove(studentUnEnrolled.CourseName);
    }  
}