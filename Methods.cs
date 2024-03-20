using Nure.NET.Parsers;
using Nure.NET.Parsers.Repair;
using Nure.NET.Types;

namespace Nure.NET;

public static class Cist
{
    public static List<Auditory>? GetAuditories()
    {
        try
        {
            return NureParser.ParseAuditories();
        }
        catch (Exception e)
        {
            throw new Exception("Error while getting auditories", e);
        }
    }
    
    public static List<Group>? GetGroups()
    {
        try
        {
            return NureParser.ParseGroups();
        }
        catch (Exception e)
        {
            throw new Exception("Error while getting groups", e);
        }
    }
    
    public static List<Teacher>? GetTeachers()
    {
        try
        {
            return NureParser.ParseTeachers();
        }
        catch (Exception e)
        {
            throw new Exception("Error while getting teachers", e);
        }
    }
    
    public static List<Event>? GetEvents(long startTime, long endTime, EventType type, long id)
    {
        var json = JsonFixers.TryFix(Requests.GetEventsJson(type, id));
        return NureParser.ParseEvents(json);
    }
}