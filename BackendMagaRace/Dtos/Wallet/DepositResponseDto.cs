using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Dtos.Wallet
{
    public class DepositResponseDto
    {
        public Guid Id { get; set; }
        public DepositMethod Method { get; set; }
        public DepositStatus Status { get; set; }
        public decimal AmountClp { get; set; }
        public decimal TransbankFee { get; set; }
        public decimal Iva { get; set; }
        public decimal TotalClp { get; set; }
        public decimal RateSnapshot { get; set; }
        public decimal ExpectedUsdt { get; set; }
        public DateTime RateExpiresAt { get; set; }
        public string? ReceiptUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
