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

    public static string GetAuditories(bool AsCist)
    {
        try
        {
            return NureParser.ParseAuditories(AsCist);
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

    public static string GetGroups(bool AsCist)
    {
        try
        {
            return NureParser.ParseGroups(AsCist);
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

    public static string GetTeachers(bool AsCist)
    {
        try
        {
            return NureParser.ParseTeachers(AsCist);
        }
        catch (Exception e)
        {
            throw new Exception("Error while getting teachers", e);
        }
    }
    
    public static List<Event>? GetEvents( EventType type, long id, long startTime = 0, long endTime = 0)
    {
        var json = JsonFixers.TryFix(Requests.GetEventsJson(type, id));
        if(startTime == 0 && endTime == 0)
            return NureParser.ParseEvents(json);
        else
        {
            var events = NureParser.ParseEvents(json);
            return events.Where(e => e.StartTime >= startTime && e.EndTime <= endTime).ToList();
        }
    }
}