using System;

namespace BackendMagaRace.Dtos.Qualifier
{
    public class QualifierSessionDto
    {
        public DateTime ActiveUntil { get; set; }
        public int? BestLapMs { get; set; }
    }
}