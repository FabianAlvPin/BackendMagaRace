using System;

namespace BackendMagaRace.Models
{
    public class QualifierSession
    {
        public Guid Id { get; set; }
        public Guid QualifierEventId { get; set; }
        public Guid UserId { get; set; }

        public DateTime ActiveUntil { get; set; }

        public int? BestLapMs { get; set; }
        public int? SecondBestLapMs { get; set; }
        public int? ThirdBestLapMs { get; set; }

        public User User { get; set; }
        public QualifierEvent Event { get; set; }
    }
}
