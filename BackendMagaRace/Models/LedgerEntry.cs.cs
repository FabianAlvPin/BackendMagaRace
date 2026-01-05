using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Models
{
    public class LedgerEntry
    {
        public Guid Id { get; set; }

        // Dueño del movimiento (CLAVE)
        public Guid UserId { get; set; }

        public LedgerType Type { get; set; }

        // Siempre positivo
        public decimal Amount { get; set; }

        // Ej: race_win, race_loss, usdt_purchase, withdraw
        public string Reference { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
