namespace BackendMagaRace.Dtos.Qualifier
{
    // Versión liviana del ranking: solo el líder (top1) y la posición/tiempo propios.
    // Pensada para sondeo periódico durante la carrera, sin pagar el costo de traer
    // y ordenar el ranking completo del evento (ver GetRanking).
    public class LeaderboardSummaryDto
    {
        public RankingItemDto? Leader { get; set; }
        public RankingItemDto? Me { get; set; }
    }
}
