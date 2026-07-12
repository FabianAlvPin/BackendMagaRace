using System;

namespace BackendMagaRace.Models
{
    public class QualifierEntry
    {
        public Guid Id { get; set; }

        public Guid QualifierEventId { get; set; }

        public Guid UserId { get; set; }

        // Valor pagado por entrar
        public decimal EntryCost { get; set; }

        // Fecha de compra
        public DateTime PurchasedAt { get; set; }

        // Hasta cuándo puede participar
        public DateTime ActiveUntil { get; set; }

        public User User { get; set; } = null!;

        public QualifierEvent Event { get; set; } = null!;
    }
}