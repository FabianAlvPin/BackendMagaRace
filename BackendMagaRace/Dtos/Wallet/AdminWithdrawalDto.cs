using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Dtos.Wallet
{
    public class AdminWithdrawalDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public WithdrawalStatus Status { get; set; }
        public decimal AmountUsdt { get; set; }
        public decimal FeeUsdt { get; set; }
        public decimal NetUsdt { get; set; }
        public decimal RateSnapshot { get; set; }
        public decimal ClpEquivalent { get; set; }
        public string Bank { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Rut { get; set; } = string.Empty;
        public string HolderName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ApproveWithdrawalDto
    {
        public string? Notes { get; set; }
    }

    public class RejectWithdrawalDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
