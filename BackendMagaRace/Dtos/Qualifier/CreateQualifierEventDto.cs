using BackendMagaRace.Models;

namespace BackendMagaRace.Dtos.Qualifier
{
    public class CreateQualifierEventDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid TrackId { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime EndsAt { get; set; }

        public int EntryCost { get; set; }

        public int BasePrize { get; set; }
        public TrackDirection Direction { get; set; }

        public CarCategory CarCategory { get; set; }

        public TransmissionType Transmission { get; set; }

        public int Laps { get; set; }
    }
}
