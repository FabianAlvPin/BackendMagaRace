namespace BackendMagaRace.Models.Enums
{
    public enum LedgerType
    {
        // === JUEGO ===
        RaceWin = 1,        // Gana créditos en carrera
        RaceLoss = 2,       // Pierde créditos en carrera
        TournamentPrize = 3,// Premio torneo

        // === COMPRAS ===
        Purchase = 10,      // Compra créditos con USDT

        // === RETIROS ===
        WithdrawRequest = 20, // Solicitud de retiro
        WithdrawApproved = 21,// Retiro aprobado
        WithdrawRejected = 22,// Retiro rechazado

        // === AJUSTES ADMIN ===
        AdminCredit = 90,   // Crédito manual
        AdminDebit = 91     // Débito manual
    }
}
