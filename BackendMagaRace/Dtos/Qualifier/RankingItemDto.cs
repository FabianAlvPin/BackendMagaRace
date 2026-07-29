namespace BackendMagaRace.Dtos.Qualifier
{
    public class RankingItemDto
    {
        public int Position { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public int BestLapMs { get; set; }
    }
}
