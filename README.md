# Nure.NET

> [!NOTE]
> This library is unofficial and not supported by cist.nure.ua administration. It is developed and maintained by students and the Mindenit Team.

> [!IMPORTANT]
> [Українська версія документації](README.ua.md)

A .NET library for interacting with NURE's schedule (cist.nure.ua). The library provides a convenient API for accessing information about schedules, teachers, groups, and university auditoriums.

![Nuget](https://img.shields.io/nuget/v/Nure.NET)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

## 📋 Table of Contents

- [Installation](#-installation)
- [Key Features](#-key-features)
- [Usage](#-usage)
    - [Getting Groups List](#getting-groups-list)
    - [Getting Teachers List](#getting-teachers-list)
    - [Getting Auditoriums List](#getting-auditoriums-list)
    - [Working with Schedule](#working-with-schedule)
- [Data Types](#-data-types)
- [Error Handling](#-error-handling)
- [Additional Features](#-additional-features)
- [Contributing](#-contributing)

## 📥 Installation

The library is available via NuGet. You can install it using one of the following methods:

Using .NET CLI:
```bash
dotnet add package Nure.NET
```

Using Package Manager Console:
```powershell
Install-Package Nure.NET
```

## 🚀 Key Features

- Retrieve list of all university groups
- Retrieve list of teachers
- Retrieve list of auditoriums
- Get schedules for groups, teachers, and auditoriums
- Support for schedule filtering by time period
- Automatic CIST server availability detection
- Handling of incorrect JSON responses from API

## 💻 Usage

### Getting Groups List

```csharp
using Nure.NET;
using Nure.NET.Types;

// Get all groups
List<Group>? groups = Cist.GetGroups();

foreach (var group in groups)
{
    Console.WriteLine($"ID: {group.Id}, Name: {group.Name}");
}

// Get data in CIST format
string cistJson = Cist.GetGroups(true);
```

### Getting Teachers List

```csharp
using Nure.NET;
using Nure.NET.Types;

// Get all teachers
List<Teacher>? teachers = Cist.GetTeachers();

foreach (var teacher in teachers)
{
    Console.WriteLine($"ID: {teacher.Id}");
    Console.WriteLine($"Full Name: {teacher.FullName}");
    Console.WriteLine($"Short Name: {teacher.ShortName}");
}
```

### Getting Auditoriums List

```csharp
using Nure.NET;
using Nure.NET.Types;

// Get all auditoriums
List<Auditory>? auditories = Cist.GetAuditories();

foreach (var auditory in auditories)
{
    Console.WriteLine($"ID: {auditory.Id}, Name: {auditory.Name}");
}
```

### Working with Schedule

```csharp
using Nure.NET;
using Nure.NET.Types;

// Get group schedule
List<Event>? groupSchedule = Cist.GetEvents(
    type: EventType.Group,
    id: 10304333  // Group ID
);

// Get schedule for specific period
List<Event>? periodSchedule = Cist.GetEvents(
    type: EventType.Group,
    id: 10304333,
    startTime: 1693170000,  // Unix timestamp start period
    endTime: 1694811599     // Unix timestamp end period
);

// Get teacher schedule
List<Event>? teacherSchedule = Cist.GetEvents(
    type: EventType.Teacher,
    id: teacherId
);

// Get auditory schedule
List<Event>? auditorySchedule = Cist.GetEvents(
    type: EventType.Auditory,
    id: auditoryId
);

// Processing the retrieved schedule
foreach (var event in groupSchedule)
{
    Console.WriteLine($"Class #{event.NumberPair}");
    Console.WriteLine($"Subject: {event.Subject?.Title}");
    Console.WriteLine($"Type: {event.Type}");
    Console.WriteLine($"Auditory: {event.Auditory}");
    Console.WriteLine($"Start: {DateTimeOffset.FromUnixTimeSeconds((long)event.StartTime)}");
    Console.WriteLine($"End: {DateTimeOffset.FromUnixTimeSeconds((long)event.EndTime)}");
    
    // Teachers
    foreach (var teacher in event.Teachers)
    {
        Console.WriteLine($"Teacher: {teacher.FullName}");
    }
    
    // Groups
    foreach (var group in event.Groups)
    {
        Console.WriteLine($"Group: {group.Name}");
    }
}
```

## 📊 Data Types

### Event
Represents one class in the schedule:
```csharp
public class Event
{
    public int? NumberPair { get; set; }        // Class number
    public Subject? Subject { get; set; }        // Subject
    public long? StartTime { get; set; }         // Start time (Unix timestamp)
    public long? EndTime { get; set; }           // End time (Unix timestamp)
    public string? Auditory { get; set; }        // Auditory
    public string? Type { get; set; }            // Class type (Lc, Pr, Lb, etc.)
    public List<Teacher>? Teachers { get; set; } // List of teachers
    public List<Group>? Groups { get; set; }     // List of groups
}
```

### Subject
Subject information:
```csharp
public class Subject
{
    public int? Id { get; set; }      // Subject ID
    public string? Title { get; set; } // Full name
    public string? Brief { get; set; } // Short name
}
```

### EventType
Entity types for schedule retrieval:
```csharp
public enum EventType
{
    Group = 1,     // Group schedule
    Teacher = 2,   // Teacher schedule
    Auditory = 3   // Auditory schedule
}
```

## ⚠️ Error Handling

The library uses exception mechanism for error handling. All methods can throw an `Exception` with detailed problem description. It's recommended to wrap method calls in try-catch blocks:

```csharp
try
{
    var groups = Cist.GetGroups();
    // Process data
}
catch (Exception e)
{
    Console.WriteLine($"Error while getting groups: {e.Message}");
}
```

## 🔧 Additional Features

### Getting Raw Data

All main methods have overloads that return data in CIST format:

```csharp
// Get groups in CIST format
string cistGroupsJson = Cist.GetGroups(true);

// Get teachers in CIST format
string cistTeachersJson = Cist.GetTeachers(true);

// Get auditoriums in CIST format
string cistAuditoriesJson = Cist.GetAuditories(true);
```

### Automatic Server Detection

The library automatically checks the availability of CIST servers (cist.nure.ua and cist2.nure.ua) and uses the one that responds faster.

## 🤝 Contributing

We welcome contributions to the library! If you found a bug or have ideas for improvements, please:

1. Fork the repository
2. Create a branch for your changes
3. Make the necessary changes
4. Create a pull request with description of changes

## 📝 License

This project is distributed under the GNU GPL v3 license. See the [LICENSE](LICENSE) file for more details.
