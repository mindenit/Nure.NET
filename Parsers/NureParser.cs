using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Nure.NET.Parsers.Repair;
using Nure.NET.Types;

namespace Nure.NET.Parsers;

public class NureParser
{
    private static string GetType(int? id)
    {
        if (id == 10 || id == 12)
        {
            return "Пз";
        }
        else if (id == 20 | id == 21 || id == 22 || id == 23 || id == 24)
        {
            return "Лб";
        }
        else if (id == 30)
        {
            return "Конс";
        }
        else if (id == 40 || id == 41)
        {
            return "Зал";
        }
        else if (id == 50 || id == 51 || id == 52 || id == 53 || id == 54 || id == 55)
        {
            return "Екз";
        }
        else if (id == 60)
        {
            return "КП/КР";
        }

        return "Лк";
    }
    
    public static List<Auditory>? ParseAuditories()
    {
        List<Auditory>? auditories = new List<Auditory>();

        var json = JsonFixers.TryFix(Requests.GetAuditoriesJson());
        auditories = RemoveDuplicateAuditories(JsonSerializer.Deserialize<List<Auditory>>(json));

        return auditories;
    }
    
    private static List<Auditory> RemoveDuplicateAuditories(List<Auditory> list)
    {
        return list.GroupBy(x => new { x.Id, x.Name })
            .Select(x => x.First())
            .ToList();
    }
    
    private static Auditory? GetAuditoryByName(List<Auditory> auditories, string name)
    {
        return auditories.FirstOrDefault(a => a.Name == name);
    }
    
    public static List<Group>? ParseGroups()
    {
        List<Group>? groups = new List<Group>();

        var json = JsonFixers.TryFix(Requests.GetGroupsJson());
        groups = RemoveDuplicateGroups(JsonSerializer.Deserialize<List<Group>>(json));
        
        return groups;
    }

    private static List<Group> RemoveDuplicateGroups(List<Group> deserialize)
    {
        return deserialize.GroupBy(x => new { x.Id, x.Name })
            .Select(x => x.First())
            .ToList();
    }
    
    public static List<Teacher>? ParseTeachers()
    {
        List<Teacher>? teachers = new List<Teacher>();

        var json = JsonFixers.TryFix(Requests.GetTeachersJson());
        teachers = RemoveDuplicateTeachers(JsonSerializer.Deserialize<List<Teacher>>(json));
        
        return teachers;
    }

    private static List<Teacher> RemoveDuplicateTeachers(List<Teacher>? deserialize)
    {
        return deserialize.GroupBy(x => new { x.Id })
            .Select(x => x.First())
            .ToList();
    }
    
    private static Subject? FindSubjectById(JsonElement subjects, int? id)
    {
        foreach (var subject in subjects.EnumerateArray())
        {
            if (subject.GetProperty("id").GetInt32() == id)
            {
                return new Subject
                {
                    Id = subject.GetProperty("id").GetInt32(),
                    Brief = subject.GetProperty("brief").GetString(),
                    Title = subject.GetProperty("title").GetString()
                };
            }
        }

        return null; // Return null if no class with the given identifier is found
    }
    
    private static Teacher? FindTeacherById(JsonElement teachers, int? id)
    {
        foreach (var teacher in teachers.EnumerateArray())
        {
            if (teacher.TryGetProperty("id", out var idProperty) &&
                teacher.TryGetProperty("short_name", out var shortNameProperty) &&
                teacher.TryGetProperty("full_name", out var fullNameProperty))
            {
                if (idProperty.ValueKind == JsonValueKind.Number &&
                    shortNameProperty.ValueKind == JsonValueKind.String &&
                    fullNameProperty.ValueKind == JsonValueKind.String)
                {
                    return new Teacher
                    {
                        Id = idProperty.GetInt64(),
                        ShortName = shortNameProperty.GetString(),
                        FullName = fullNameProperty.GetString()
                    };
                }
            }
        }

        return null; // Return null if no teacher with the given identifier is found
    }
    
    private static Group? FindGroupById(JsonElement groups, int? id)
    {
        foreach (var group in groups.EnumerateArray())
        {
            if (group.TryGetProperty("id", out var idProperty) &&
                group.TryGetProperty("name", out var nameProperty))
            {
                if (idProperty.ValueKind == JsonValueKind.Number &&
                    nameProperty.ValueKind == JsonValueKind.String)
                {
                    return new Group
                    {
                        Id = idProperty.GetInt64(),
                        Name = nameProperty.GetString()
                    };
                }
            }
        }

        return null; // Return null if no group with the given identifier is found
    }
    
    
    
    public static List<Event> ParseEvents(string json)
    {
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var events = data.GetProperty("events");

        List<Event> pairs = new List<Event>();

        foreach (var lesson in events.EnumerateArray())
        {
            Event pair = new Event();
            pair.numberPair = lesson.GetProperty("number_pair").GetInt32();
            pair.startTime = lesson.GetProperty("start_time").GetInt64();
            pair.endTime = lesson.GetProperty("end_time").GetInt64();
            pair.type = GetType(lesson.GetProperty("type").GetInt32());

            pair.auditory = lesson.GetProperty("auditory").GetString();

            pair.subject = FindSubjectById(events.GetProperty("subjects"), lesson.GetProperty("subject_id").GetInt32());

            if (lesson.TryGetProperty("teachers", out var teachersProperty) && teachersProperty.GetArrayLength() > 0)
            {
                pair.teachers = new List<Teacher>();
                foreach (var teacher in teachersProperty.EnumerateArray())
                {
                    pair.teachers.Add(FindTeacherById(events.GetProperty("teachers"), teacher.GetInt32()));
                }
            }
            else
            {
                pair.teachers = new List<Teacher>();
            }

            if (lesson.TryGetProperty("groups", out var groupsProperty) && groupsProperty.GetArrayLength() > 0)
            {
                pair.groups = new List<Group>();
                foreach (var group in groupsProperty.EnumerateArray())
                {
                    var foundGroup = FindGroupById(events.GetProperty("groups"), group.GetInt32());
                    pair.groups.Add(foundGroup);
                }
            }
            else
            {
                pair.groups = new List<Group>();
            }

            pairs.Add(pair);
        }

        return pairs.OrderBy(x => x.startTime).ToList();
    }
}