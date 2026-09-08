using System;

namespace BackendMagaRace.Models
{
    // Cuenta bancaria CLP a la que se abonan los retiros del usuario (una por usuario, editable)
    public class WithdrawalAccount
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Bank { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Rut { get; set; } = string.Empty;
        public string HolderName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
