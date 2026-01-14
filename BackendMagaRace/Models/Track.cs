namespace BackendMagaRace.Models
{
    public class Track
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }

        public ICollection<LapTime> LapTimes { get; set; } = new List<LapTime>();
    }

}
