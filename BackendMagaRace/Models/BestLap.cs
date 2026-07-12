namespace BackendMagaRace.Models
{
    public class BestLap
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public int LapTimeMs { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
