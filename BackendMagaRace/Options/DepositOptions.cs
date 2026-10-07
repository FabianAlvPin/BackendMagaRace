namespace BackendMagaRace.Options
{
    public class DepositOptions
    {
        // Monto minimo permitido por solicitud de deposito por transferencia bancaria
        public decimal MinAmountClp { get; set; } = 1000m;
    }
}
