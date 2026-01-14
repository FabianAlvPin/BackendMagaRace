using System;
using System.Collections.Generic;

namespace BackendMagaRace.Models
{
    public class QualifierEvent
    {
        public Guid Id { get; set; }
        public Guid TrackId { get; set; }

        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }

        public int EntryCost { get; set; }
        public int PrizePool { get; set; }

        public bool IsClosed { get; set; }

        public Track Track { get; set; }
        public ICollection<QualifierSession> Sessions { get; set; }
    }
}
