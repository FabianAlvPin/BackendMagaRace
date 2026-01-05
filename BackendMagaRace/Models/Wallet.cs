using System;

namespace BackendMagaRace.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }

        // Relación 1:1
        public Guid UserId { get; set; }

        // Cache de saldo (derivado del ledger)
        public decimal Balance { get; set; } = 0;

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public User User { get; set; } = null!;
    }
}

