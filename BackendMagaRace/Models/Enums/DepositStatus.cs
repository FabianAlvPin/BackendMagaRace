namespace BackendMagaRace.Models.Enums
{
    public enum DepositStatus
    {
        // Transbank: creado, esperando que el usuario pague
        Pending = 0,

        // Transferencia: esperando comprobante o revisión manual del admin
        PendingReview = 1,

        Completed = 2,
        Rejected = 3,

        // El rate congelado venció antes de completarse
        Expired = 4
    }
}
