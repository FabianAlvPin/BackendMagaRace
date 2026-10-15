using System;

namespace BackendMagaRace.Models
{
    public class DuelParticipant
    {
        public Guid Id { get; set; }

        public Guid DuelId { get; set; }
        public Guid UserId { get; set; }

        // Se llenan recién al liquidar (Settle), quedan null/false mientras el duelo
        // está en curso (Status = Locked).
        public int? FinalPosition { get; set; }
        public bool Disconnected { get; set; }
        public decimal PrizeUsdt { get; set; }

        public Duel Duel { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
