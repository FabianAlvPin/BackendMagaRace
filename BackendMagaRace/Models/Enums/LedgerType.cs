namespace BackendMagaRace.Models.Enums
{
    public enum LedgerType
    {
        // === JUEGO ===
        RaceWin = 1,        // Gana créditos en carrera
        RaceLoss = 2,       // Pierde créditos en carrera
        TournamentPrize = 3,// Premio torneo
        EventPrize = 4,     // Premio por posición en evento semanal (time attack / QualifierEvent)
        EventEntryFee = 5,  // Cobro de entrada a un QualifierEvent

        // === DUELOS (Free World) ===
        DuelWager = 30,     // Apuesta debitada al entrar a un duelo
        DuelPrize = 31,     // Premio acreditado al repartir el pozo de un duelo

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
