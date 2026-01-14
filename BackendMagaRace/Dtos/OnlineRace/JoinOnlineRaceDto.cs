using System;

namespace BackendMagaRace.Dtos.OnlineRace
{
    public class JoinOnlineRaceDto
    {
        public Guid RaceId { get; set; }
        public Guid UserId { get; set; }
    }
}
