namespace BackendMagaRace.Dtos.OnlineRace
{
    public class SubmitLapDto
    {
        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public int LapTimeMs { get; set; }
    }
}
