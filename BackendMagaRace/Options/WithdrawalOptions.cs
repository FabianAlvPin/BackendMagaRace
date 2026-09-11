namespace BackendMagaRace.Options
{
    public class WithdrawalOptions
    {
        // Comision que se retiene sobre cada retiro (ganancia de la casa)
        public decimal FeeRate { get; set; } = 0.02m;

        // Monto minimo permitido por solicitud de retiro
        public decimal MinAmountUsdt { get; set; } = 10m;
    }
}
