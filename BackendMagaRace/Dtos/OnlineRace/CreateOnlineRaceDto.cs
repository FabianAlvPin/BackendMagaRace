namespace BackendMagaRace.Dtos.OnlineRace
{
    public class CreateOnlineRaceDto
    {
        public Guid TrackId { get; set; }
        public int EntryCost { get; set; }
        public int MaxPlayers { get; set; }
        public int Laps { get; set; }
    }

}
