namespace BackendMagaRace.Dtos.OnlineRace
{
    public class CreateOnlineRaceDto
    {
        public Guid TrackId { get; set; }
        public Guid OwnerUserId { get; set; }
        public string RoomName { get; set; }
        public int EntryCost { get; set; }
        public int MaxPlayers { get; set; }
        public int Laps { get; set; }
    }

}
