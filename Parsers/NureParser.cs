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
        if (id is 10 or 12)
        {
            return "Пз";
        }
        else if (id == 20 || id == 21 || id == 22 || id == 23 || id == 24)
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

        var json = Requests.GetAuditoriesJson();
        
        var cistAuditories = JsonSerializer.Deserialize<JsonElement>(json);

        if (cistAuditories.TryGetProperty("university", out var university) &&
            university.TryGetProperty("buildings", out var buildings))
        {
            foreach (var building in buildings.EnumerateArray())
            {
                if (building.TryGetProperty("auditories", out var auditoriesArray))
                {
                    foreach (var auditoryElement in auditoriesArray.EnumerateArray())
                    {
                        var auditory = new Auditory
                        {
                            Id = long.Parse(auditoryElement.GetProperty("id").GetString()),
                            Name = auditoryElement.GetProperty("short_name").GetString(),
                        };
                        auditories.Add(auditory);
                    }
                }
            }
        }

        return RemoveDuplicateAuditories(auditories);
    }
    
    public static string ParseAuditories(bool asCist)
    {
        if (asCist)
        {
            var json = Requests.GetAuditoriesJson();
            return json;
        }
        return "";
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
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        if (data.TryGetProperty("university", out var university) &&
            university.TryGetProperty("faculties", out var faculties))
        {
            foreach (var faculty in faculties.EnumerateArray())
            {
                if (faculty.TryGetProperty("directions", out var directions))
                {
                    foreach (var direction in directions.EnumerateArray())
                    {
                        if (direction.TryGetProperty("groups", out var groupsArray))
                        {
                            foreach (var groupElement in groupsArray.EnumerateArray())
                            {
                                var group = new Group
                                {
                                    Id = groupElement.GetProperty("id").GetInt32(),
                                    Name = groupElement.GetProperty("name").GetString(),
                                };
                                groups.Add(group);
                            }
                        }

                        if (direction.TryGetProperty("specialities", out var specialities))
                        {
                            foreach (var speciality in specialities.EnumerateArray())
                            {
                                if (speciality.TryGetProperty("groups", out var specialityGroups))
                                {
                                    foreach (var groupElement in specialityGroups.EnumerateArray())
                                    {
                                        var group = new Group
                                        {
                                            Id = groupElement.GetProperty("id").GetInt32(),
                                            Name = groupElement.GetProperty("name").GetString(),
                                        };
                                        groups.Add(group);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        return RemoveDuplicateGroups(groups);
    }

    public static string ParseGroups(bool asCist)
    {
        if (asCist)
        {
            var json = Requests.GetGroupsJson();
            return json;
        }

        return "";
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
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        if (data.TryGetProperty("university", out var university) &&
            university.TryGetProperty("faculties", out var faculties))
        {
            foreach (var faculty in faculties.EnumerateArray())
            {
                if (faculty.TryGetProperty("departments", out var departments))
                {
                    foreach (var department in departments.EnumerateArray())
                    {
                        if (department.TryGetProperty("teachers", out var teachersArray))
                        {
                            foreach (var teacherElement in teachersArray.EnumerateArray())
                            {
                                var teacher = new Teacher
                                {
                                    Id = teacherElement.GetProperty("id").GetInt32(),
                                    FullName = teacherElement.GetProperty("full_name").GetString(),
                                    ShortName = teacherElement.GetProperty("short_name").GetString(),
                                };
                                teachers.Add(teacher);
                            }
                        }

                        if (department.TryGetProperty("departments", out var childDepartments))
                        {
                            foreach (var childDepartment in childDepartments.EnumerateArray())
                            {
                                if (childDepartment.TryGetProperty("teachers", out var childTeachers))
                                {
                                    foreach (var teacherElement in childTeachers.EnumerateArray())
                                    {
                                        var teacher = new Teacher
                                        {
                                            Id = teacherElement.GetProperty("id").GetInt32(),
                                            FullName = teacherElement.GetProperty("full_name").GetString(),
                                            ShortName = teacherElement.GetProperty("short_name").GetString(),
                                        };
                                        teachers.Add(teacher);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return RemoveDuplicateTeachers(teachers);
    }

    public static string ParseTeachers(bool asCist)
    {
        if (asCist)
        {
            var json = Requests.GetTeachersJson();
            return json;
        }
        return "";
    }

    private static List<Teacher> RemoveDuplicateTeachers(List<Teacher>? deserialize)
    {
        return deserialize.GroupBy(x => new { x.Id })
            .Select(x => x.First())
            .ToList();
    }

    private static Subject? FindSubjectById(JsonArray subjects, int? id)
    {
        foreach (var subject in subjects)
        {
            if (subject is JsonObject subjectObj &&
                subjectObj["id"]?.GetValue<int>() == id)
            {
                return new Subject()
                {
                    Id = subjectObj["id"]!.GetValue<int>(),
                    Brief = subjectObj["brief"]?.GetValue<string>(),
                    Title = subjectObj["title"]?.GetValue<string>()
                };
            }
        }

        return null; // якщо предмет з таким ідентифікатором не знайдено
    }


    private static Teacher? FindTeacherById(JsonArray teachers, int? id)
    {
        foreach (var teacher in teachers)
        {
            if (teacher is JsonObject teacherObj &&
                teacherObj["id"]?.GetValue<int>() == id &&
                teacherObj["id"]?.GetValue<long?>() is not null &&
                teacherObj["short_name"] is not null &&
                teacherObj["full_name"] is not null)
            {
                return new Teacher()
                {
                    Id = teacherObj["id"]!.GetValue<long>(),
                    ShortName = teacherObj["short_name"]!.GetValue<string>(),
                    FullName = teacherObj["full_name"]!.GetValue<string>()
                };
            }
        }

        return null; // Повертаємо null, якщо вчителя не знайдено
    }

    private static Group? FindGroupById(JsonArray groups, int? id)
    {
        foreach (var group in groups)
        {
            if (group is JsonObject groupObj &&
                groupObj["id"]?.GetValue<long>() == id &&
                groupObj["name"] is not null)
            {
                return new Group
                {
                    Id = groupObj["id"]!.GetValue<long>(),
                    Name = groupObj["name"]!.GetValue<string>()
                };
            }
        }

        return null; // Повертаємо null, якщо групу з таким ідентифікатором не знайдено
    }

    public static List<Event>? ParseEvents(string json)
    {
        try
        {
            var data = JsonNode.Parse(json)?.AsObject();
            if (data == null || !data.ContainsKey("events"))
                return null;

            var events = data["events"]?.AsArray();
            var subjects = data["subjects"]?.AsArray();
            var teachers = data["teachers"]?.AsArray();
            var groups = data["groups"]?.AsArray();

            List<Event> pairs = new List<Event>();

            if (events != null)
            {
                foreach (var lesson in events)
                {
                    if (lesson is not JsonObject lessonObj) continue;

                    Event pair = new Event
                    {
                        NumberPair = lessonObj["number_pair"]?.GetValue<int>() ?? 0,
                        StartTime = lessonObj["start_time"]?.GetValue<long>() ?? 0,
                        EndTime = lessonObj["end_time"]?.GetValue<long>() ?? 0,
                        Type = GetType(lessonObj["type"]?.GetValue<int>() ?? 0),
                        Auditory = lessonObj["auditory"]?.GetValue<string>()
                    };

                    if (subjects != null && lessonObj["subject_id"]?.GetValue<int>() is int subjectId)
                    {
                        pair.Subject = FindSubjectById(subjects, subjectId);
                    }

                    if (lessonObj["teachers"] is JsonArray teachersProperty)
                    {
                        pair.Teachers = new List<Teacher>();
                        foreach (var teacher in teachersProperty)
                        {
                            if (teacher.GetValue<int?>() is int teacherId && teachers != null)
                            {
                                var foundTeacher = FindTeacherById(teachers, teacherId);
                                if (foundTeacher != null)
                                {
                                    pair.Teachers.Add(foundTeacher);
                                }
                            }
                        }
                    }
                    else
                    {
                        pair.Teachers = new List<Teacher>();
                    }

                    if (lessonObj["groups"] is JsonArray groupsProperty)
                    {
                        pair.Groups = new List<Group>();
                        foreach (var group in groupsProperty)
                        {
                            if (group.GetValue<int?>() is int groupId && groups != null)
                            {
                                var foundGroup = FindGroupById(groups, groupId);
                                if (foundGroup != null)
                                {
                                    pair.Groups.Add(foundGroup);
                                }
                            }
                        }
                    }
                    else
                    {
                        pair.Groups = new List<Group>();
                    }

                    pairs.Add(pair);
                }
            }

            return pairs.OrderBy(x => x.StartTime).ToList();
        }
        catch (Exception)
        {
            return null;
        }
    }
}
