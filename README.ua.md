# Nure.NET

> [!NOTE]
> Ця бібліотека не є офіційною та не підтримується адміністрацією cist.nure.ua. Вона розроблена та підтримується студентами та командою Mindenit.

.NET бібліотека для взаємодії з розкладом ХНУРЕ (cist.nure.ua). Бібліотека надає зручний API для отримання інформації про розклад, викладачів, групи та аудиторії університету.

![Nuget](https://img.shields.io/nuget/v/Nure.NET)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

## 📋 Зміст

- [Встановлення](#встановлення)
- [Основні можливості](#основні-можливості)
- [Використання](#використання)
    - [Отримання списку груп](#отримання-списку-груп)
    - [Отримання списку викладачів](#отримання-списку-викладачів)
    - [Отримання списку аудиторій](#отримання-списку-аудиторій)
    - [Робота з розкладом](#робота-з-розкладом)
- [Типи даних](#типи-даних)
- [Обробка помилок](#обробка-помилок)
- [Додаткові можливості](#додаткові-можливості)
- [Внесок у розробку](#внесок-у-розробку)

## 📥 Встановлення

Бібліотека доступна через NuGet. Ви можете встановити її одним з наступних способів:

Через .NET CLI:
```bash
dotnet add package Nure.NET
```

Через Package Manager Console:
```powershell
Install-Package Nure.NET
```

## 🚀 Основні можливості

- Отримання списку всіх груп університету
- Отримання списку викладачів
- Отримання списку аудиторій
- Отримання розкладу для груп, викладачів та аудиторій
- Підтримка фільтрації розкладу за часовим проміжком
- Автоматичне визначення доступного серверу CIST
- Обробка некоректних JSON відповідей від API

## 💻 Використання

### Отримання списку груп

```csharp
using Nure.NET;
using Nure.NET.Types;

// Отримання всіх груп
List<Group>? groups = Cist.GetGroups();

foreach (var group in groups)
{
    Console.WriteLine($"ID: {group.Id}, Назва: {group.Name}");
}

// Отримання даних у форматі CIST
string cistJson = Cist.GetGroups(true);
```

### Отримання списку викладачів

```csharp
using Nure.NET;
using Nure.NET.Types;

// Отримання всіх викладачів
List<Teacher>? teachers = Cist.GetTeachers();

foreach (var teacher in teachers)
{
    Console.WriteLine($"ID: {teacher.Id}");
    Console.WriteLine($"ПІБ: {teacher.FullName}");
    Console.WriteLine($"Скорочене ім'я: {teacher.ShortName}");
}
```

### Отримання списку аудиторій

```csharp
using Nure.NET;
using Nure.NET.Types;

// Отримання всіх аудиторій
List<Auditory>? auditories = Cist.GetAuditories();

foreach (var auditory in auditories)
{
    Console.WriteLine($"ID: {auditory.Id}, Назва: {auditory.Name}");
}
```

### Робота з розкладом

```csharp
using Nure.NET;
using Nure.NET.Types;

// Отримання розкладу групи
List<Event>? groupSchedule = Cist.GetEvents(
    type: EventType.Group,
    id: 10304333  // ID групи
);

// Отримання розкладу за певний період
List<Event>? periodSchedule = Cist.GetEvents(
    type: EventType.Group,
    id: 10304333,
    startTime: 1693170000,  // Unix timestamp початку періоду
    endTime: 1694811599     // Unix timestamp кінця періоду
);

// Отримання розкладу викладача
List<Event>? teacherSchedule = Cist.GetEvents(
    type: EventType.Teacher,
    id: teacherId
);

// Отримання розкладу аудиторії
List<Event>? auditorySchedule = Cist.GetEvents(
    type: EventType.Auditory,
    id: auditoryId
);

// Обробка отриманого розкладу
foreach (var event in groupSchedule)
{
    Console.WriteLine($"Пара №{event.NumberPair}");
    Console.WriteLine($"Предмет: {event.Subject?.Title}");
    Console.WriteLine($"Тип: {event.Type}");
    Console.WriteLine($"Аудиторія: {event.Auditory}");
    Console.WriteLine($"Початок: {DateTimeOffset.FromUnixTimeSeconds((long)event.StartTime)}");
    Console.WriteLine($"Кінець: {DateTimeOffset.FromUnixTimeSeconds((long)event.EndTime)}");
    
    // Викладачі
    foreach (var teacher in event.Teachers)
    {
        Console.WriteLine($"Викладач: {teacher.FullName}");
    }
    
    // Групи
    foreach (var group in event.Groups)
    {
        Console.WriteLine($"Група: {group.Name}");
    }
}
```

## 📊 Типи даних

### Event
Представляє одну пару в розкладі:
```csharp
public class Event
{
    public int? NumberPair { get; set; }        // Номер пари
    public Subject? Subject { get; set; }        // Предмет
    public long? StartTime { get; set; }         // Час початку (Unix timestamp)
    public long? EndTime { get; set; }           // Час кінця (Unix timestamp)
    public string? Auditory { get; set; }        // Аудиторія
    public string? Type { get; set; }            // Тип пари (Лк, Пз, Лб, etc.)
    public List<Teacher>? Teachers { get; set; } // Список викладачів
    public List<Group>? Groups { get; set; }     // Список груп
}
```

### Subject
Інформація про предмет:
```csharp
public class Subject
{
    public int? Id { get; set; }      // ID предмета
    public string? Title { get; set; } // Повна назва
    public string? Brief { get; set; } // Коротка назва
}
```

### EventType
Типи сутностей для отримання розкладу:
```csharp
public enum EventType
{
    Group = 1,     // Розклад групи
    Teacher = 2,   // Розклад викладача
    Auditory = 3   // Розклад аудиторії
}
```

## ⚠️ Обробка помилок

Бібліотека використовує механізм винятків для обробки помилок. Всі методи можуть викинути `Exception` з детальним описом проблеми. Рекомендується обгортати виклики методів у try-catch блоки:

```csharp
try
{
    var groups = Cist.GetGroups();
    // Обробка даних
}
catch (Exception e)
{
    Console.WriteLine($"Помилка при отриманні груп: {e.Message}");
}
```

## 🔧 Додаткові можливості

### Отримання "сирих" даних

Для всіх основних методів існують перевантаження, які повертають дані у форматі CIST:

```csharp
// Отримання груп у форматі CIST
string cistGroupsJson = Cist.GetGroups(true);

// Отримання викладачів у форматі CIST
string cistTeachersJson = Cist.GetTeachers(true);

// Отримання аудиторій у форматі CIST
string cistAuditoriesJson = Cist.GetAuditories(true);
```

### Автоматичне визначення серверу

Бібліотека автоматично перевіряє доступність серверів CIST (cist.nure.ua та cist2.nure.ua) і використовує той, який відповідає швидше.

## 🤝 Внесок у розробку

Ми вітаємо внесок у розвиток бібліотеки! Якщо ви знайшли помилку або маєте ідеї щодо покращення, будь ласка:

1. Створіть форк репозиторію
2. Створіть гілку для ваших змін
3. Внесіть необхідні зміни
4. Створіть пул-реквест з описом змін

## 📝 Ліцензія

Цей проєкт розповсюджується під ліцензією GNU GPL v3. Детальніше дивіться у файлі [LICENSE](LICENSE).