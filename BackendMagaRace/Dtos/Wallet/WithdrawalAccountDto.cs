namespace BackendMagaRace.Dtos.Wallet
{
    public class SetWithdrawalAccountDto
    {
        public string Bank { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Rut { get; set; } = string.Empty;
        public string HolderName { get; set; } = string.Empty;
    }

    public class WithdrawalAccountResponseDto
    {
        public string Bank { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Rut { get; set; } = string.Empty;
        public string HolderName { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
