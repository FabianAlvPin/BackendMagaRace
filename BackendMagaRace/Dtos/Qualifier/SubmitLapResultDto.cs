namespace BackendMagaRace.Dtos.Qualifier
{
    public class SubmitLapResultDto
    {
        public bool IsNewRecord { get; set; }

        public int? BestLapTimeMs { get; set; }

        public int? PlayerPosition { get; set; }
    }
}
