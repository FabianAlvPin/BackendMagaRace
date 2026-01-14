using System;

namespace BackendMagaRace.Models
{
    public class OnlineRacePlayer
    {
        public Guid Id { get; set; }
        public Guid OnlineRaceId { get; set; }
        public Guid UserId { get; set; }

        public int? TotalTimeMs { get; set; }
        public int Position { get; set; }

        public User User { get; set; } = null!;
        public OnlineRace OnlineRace { get; set; } = null!;
    }
}
