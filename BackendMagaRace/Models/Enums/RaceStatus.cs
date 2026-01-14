namespace BackendMagaRace.Models
{
    public enum RaceStatus
    {
        Waiting = 0,     // Sala creada, lobby, práctica libre
        InProgress = 1,  // Carrera iniciada
        Finished = 2,    // Carrera terminada correctamente
        Cancelled = 3    // Cancelada (nadie terminó / todos salieron)
    }
}
