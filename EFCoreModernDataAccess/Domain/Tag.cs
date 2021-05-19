using System.Collections.Generic;

namespace EFModernDA.Domain
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Speaker> Speakers { get; set; }
        public IList<Session> Sessions { get; set; }

        public override string ToString() =>
            $"Tag '{Name}' tagged by {Speakers?.Count} speakers and {Sessions?.Count} sessions";
    }
}
