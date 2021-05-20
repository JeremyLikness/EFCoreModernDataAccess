using System;
using System.Collections.Generic;

namespace EFModernDA.Domain
{
    public class Session
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset SessionStart { get; set; }
        public DateTimeOffset SessionEnd { get; set; }
        public IList<Speaker> Speakers { get; set; }
        public IList<Attendee> Attendees { get; set; }
        public IList<Tag> Tags { get; set; }

        public override string ToString() =>
            $"Session {Id}: {Name} [{SessionStart} to {SessionEnd}]";
    }
}
