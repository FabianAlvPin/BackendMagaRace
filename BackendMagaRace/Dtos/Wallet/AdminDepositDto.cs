using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Dtos.Wallet
{
    public class AdminDepositDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DepositStatus Status { get; set; }
        public decimal AmountClp { get; set; }
        public decimal TotalClp { get; set; }
        public decimal ExpectedUsdt { get; set; }
        public decimal RateSnapshot { get; set; }
        public DateTime RateExpiresAt { get; set; }
        public string? ReceiptUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ApproveDepositDto
    {
        public string? Notes { get; set; }
    }

    public class RejectDepositDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
