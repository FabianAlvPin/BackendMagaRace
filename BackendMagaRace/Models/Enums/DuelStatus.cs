namespace BackendMagaRace.Models.Enums
{
    public enum DuelStatus
    {
        Locked = 0,     // Apuestas ya debitadas a todos los participantes, carrera en curso
        Finished = 1,   // Resultados reportados y pozo repartido
        Cancelled = 2   // No llegó a cobrarse nada (ej. nadie aceptó el desafío)
    }
}
