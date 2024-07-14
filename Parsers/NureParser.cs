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

        return null; // Return null if no subject with the given identifier is found
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
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(json);
            var events = data.GetProperty("events");

            List<Event> pairs = new List<Event>();

            foreach (var lesson in events.EnumerateArray())
            {
                Event pair = new Event();
                pair.NumberPair = lesson.GetProperty("number_pair").GetInt32();
                pair.StartTime = lesson.GetProperty("start_time").GetInt64();
                pair.EndTime = lesson.GetProperty("end_time").GetInt64();
                pair.Type = GetType(lesson.GetProperty("type").GetInt32());

                pair.Auditory = lesson.GetProperty("auditory").GetString();

                pair.Subject = FindSubjectById(data.GetProperty("subjects"), lesson.GetProperty("subject_id").GetInt32());

                if (lesson.TryGetProperty("teachers", out var teachersProperty) && teachersProperty.GetArrayLength() > 0)
                {
                    pair.Teachers = new List<Teacher>();
                    foreach (var teacher in teachersProperty.EnumerateArray())
                    {
                        pair.Teachers.Add(FindTeacherById(data.GetProperty("teachers"), teacher.GetInt32()));
                    }
                }
                else
                {
                    pair.Teachers = new List<Teacher>();
                }

                if (lesson.TryGetProperty("groups", out var groupsProperty) && groupsProperty.GetArrayLength() > 0)
                {
                    pair.Groups = new List<Group>();
                    foreach (var group in groupsProperty.EnumerateArray())
                    {
                        var foundGroup = FindGroupById(data.GetProperty("groups"), group.GetInt32());
                        pair.Groups.Add(foundGroup);
                    }
                }
                else
                {
                    pair.Groups = new List<Group>();
                }

                pairs.Add(pair);
            }

            return pairs.OrderBy(x => x.StartTime).ToList();
        }
        catch (Exception)
        {
            return null;
        }
        
    }
}
