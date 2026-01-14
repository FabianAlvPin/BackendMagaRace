using System;

namespace BackendMagaRace.Dtos.Qualifier
{
    public class JoinQualifierDto
    {
        public Guid UserId { get; set; }
        public Guid QualifierEventId { get; set; }
    }
}
