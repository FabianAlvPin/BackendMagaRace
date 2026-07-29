namespace BackendMagaRace.Dtos.Qualifier
{
    public class SubmitLapResponse
    {
        public RankingItemDto Leader { get; set; }
        public RankingItemDto Me { get; set; }
    }
}
