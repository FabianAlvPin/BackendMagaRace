using System;
using System.Collections.Generic;

namespace BackendMagaRace.Models
{
    public class OnlineRace
    {
        public Guid Id { get; set; }
        public Guid TrackId { get; set; }
        public string RoomName { get; set; } = null!;
        public int EntryCost { get; set; }
        public int MaxPlayers { get; set; }
        public int Laps { get; set; }

        public RaceStatus Status { get; set; }
        public ICollection<OnlineRacePlayer> Players { get; set; }
          = new List<OnlineRacePlayer>();
    }
}
