namespace BackendMagaRace.Models.Enums
{
    public enum WithdrawalStatus
    {
        // Solicitado, saldo ya retenido, esperando que el admin haga el P2P + transferencia
        Pending = 0,

        // Admin pagó manualmente al usuario
        Approved = 1,

        // Admin rechazó, saldo retenido devuelto
        Rejected = 2
    }
}
