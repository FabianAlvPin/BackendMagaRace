namespace BackendMagaRace.Models
{
    public class Track
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }

        // Nombre exacto de la escena de Unity a cargar para correr en esta pista
        // (ej. "qualy", "Nurburgring") — cada pista nueva es una escena distinta.
        public string SceneName { get; set; } = null!;

        public ICollection<LapTime> LapTimes { get; set; } = new List<LapTime>();
    }

}
