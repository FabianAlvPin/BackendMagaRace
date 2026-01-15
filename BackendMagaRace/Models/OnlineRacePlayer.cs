using System;

namespace BackendMagaRace.Models
{
    public class OnlineRacePlayer
    {
        public Guid Id { get; set; }

        public Guid OnlineRaceId { get; set; }
        public OnlineRace OnlineRace { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        // --- CONTROL DE CARRERA ---
        public bool IsOwner { get; set; }              // creador de la carrera
        public DateTime JoinedAt { get; set; }         // cuando entró
        public bool IsConnected { get; set; } = true;  // estado actual

        // --- RESULTADOS ---
        public int? TotalTimeMs { get; set; }
        public int? Position { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}
