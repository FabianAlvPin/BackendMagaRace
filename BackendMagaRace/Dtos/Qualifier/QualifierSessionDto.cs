using System;

namespace BackendMagaRace.Dtos.Qualifier
{
    public class QualifierSessionDto
    {
        public Guid Id { get; set; }
        public DateTime ActiveUntil { get; set; }
        public int? BestLapMs { get; set; }
    }
}