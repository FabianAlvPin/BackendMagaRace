namespace BackendMagaRace.Options
{
    public class DuelOptions
    {
        // Las 3 apuestas fijas que se ofrecen al desafiar (baja/media/alta).
        public decimal LowWagerUsdt { get; set; } = 0.5m;
        public decimal MidWagerUsdt { get; set; } = 1.5m;
        public decimal HighWagerUsdt { get; set; } = 3m;

        // Comision que se retiene sobre el pozo total (ganancia de la casa).
        public decimal FeeRate { get; set; } = 0.02m;

        // Tope de participantes por desafío (incluye al que desafía).
        public int MaxParticipants { get; set; } = 6;
    }
}
