using System;
using System.Collections.Generic;

namespace EFModernDA.Domain
{
    public class Attendee : Participant
    {
        public DateTimeOffset DateRegistered { get; set; }
        public bool ConsentedToPhotographs { get; set; }
        public IList<Session> Sessions { get; set; }

        public override string ToString()
        {
            var consent = ConsentedToPhotographs ? "Yes" : "No";
            return $"Attendee {Name} Consented: {consent}";
        }
    }
}
