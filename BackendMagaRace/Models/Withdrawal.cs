using System;
using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Models
{
    public class Withdrawal
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public WithdrawalStatus Status { get; set; } = WithdrawalStatus.Pending;

        // Monto solicitado por el usuario; es lo que se retiene de su wallet
        public decimal AmountUsdt { get; set; }

        // Comision retenida (ganancia) y neto efectivamente pagado (AmountUsdt - FeeUsdt)
        public decimal FeeUsdt { get; set; }
        public decimal NetUsdt { get; set; }

        // Tipo de cambio USDT/CLP congelado al solicitar el retiro
        public decimal RateSnapshot { get; set; }

        // Calculado sobre NetUsdt (lo que efectivamente se le transfiere en CLP al usuario)
        public decimal ClpEquivalent { get; set; }

        // Copia de los datos bancarios al momento del retiro (no FK: si el usuario
        // cambia su WithdrawalAccount después, esto no debe verse afectado)
        public string Bank { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Rut { get; set; } = string.Empty;
        public string HolderName { get; set; } = string.Empty;

        public Guid? ReviewedByAdminId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? AdminNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
