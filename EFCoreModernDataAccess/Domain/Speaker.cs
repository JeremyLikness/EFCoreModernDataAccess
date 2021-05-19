using System.Collections.Generic;

namespace EFModernDA.Domain
{
    public class Speaker : Participant
    {
        public string Bio { get; set; }
        public IList<Session> Presentations { get; set; }
        public IList<Tag> Tags { get; set; }

        public override string ToString() =>
            $"Speaker {Name} ({Presentations?.Count} sessions)";
    }
}
