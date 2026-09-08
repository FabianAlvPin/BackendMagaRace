namespace BackendMagaRace.Options
{
    public class TransbankOptions
    {
        public string CommerceCode { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public bool IsProduction { get; set; } = false;

        // Comisión que se le pasa al cliente + IVA sobre esa comisión
        public decimal CreditFeeRate { get; set; } = 0.0235m;
        public decimal DebitFeeRate { get; set; } = 0.0175m;
        public decimal IvaRate { get; set; } = 0.19m;
    }
}
