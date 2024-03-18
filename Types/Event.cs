namespace Nure.NET.Types;


    public class Event
    {
        public int? numberPair { get; set; }
        public Subject? subject { get; set; }
        public long? startTime { get; set; }
        public long? endTime { get; set; }
        public string? auditory { get; set; }
        public string? type { get; set; }
        public List<Teacher>? teachers { get; set; } = new List<Teacher>();
        public List<Group>? groups { get; set; } = new List<Group>();
    }

    public class Subject
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Brief { get; set; }
    }

