using System;

namespace BackendMagaRace.Dtos.Qualifier
{
    public class SubmitQualifierLapDto
    {
        public Guid UserId { get; set; }

        public Guid QualifierEventId { get; set; }

        public int TimeMs { get; set; }
    }
}