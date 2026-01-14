using System;

namespace BackendMagaRace.Models
{
    public class LapTime
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TrackId { get; set; }

        public int TimeMs { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Track Track { get; set; }
    }
}
