using System;

namespace BackendMagaRace.Models
{
    public class QualifierPrize
    {
        public Guid Id { get; set; }

        public Guid QualifierEventId { get; set; }

        public int FromPosition { get; set; }

        public int ToPosition { get; set; }

        // Premio fijo
        public decimal? FixedAmount { get; set; }

        // Porcentaje del pozo acumulado
        public decimal? PrizePercent { get; set; }

        public QualifierEvent Event { get; set; } = null!;
    }
}