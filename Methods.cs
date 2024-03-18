using Nure.NET.Parsers;
using Nure.NET.Parsers.Repair;
using Nure.NET.Types;

namespace Nure.NET;

public class Nure
{
    public static List<Auditory>? GetAuditories()
    {
        return NureParser.ParseAuditories();
    }
    
    public static List<Group>? GetGroups()
    {
        return NureParser.ParseGroups();
    }
    
    public static List<Teacher>? GetTeachers()
    {
        return NureParser.ParseTeachers();
    }
    
    public static List<Event>? GetEvents(long startTime, long endTime, EventType type, long id)
    {
        var json = JsonFixers.TryFix(Requests.GetEventsJson(type, id));
        return NureParser.ParseEvents(json);
    }
}