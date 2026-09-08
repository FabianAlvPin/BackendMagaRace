using System;
using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Models
{
    public class Deposit
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DepositMethod Method { get; set; }
        public DepositStatus Status { get; set; } = DepositStatus.Pending;

        // Monto que el usuario quiere convertir a USDT, antes de recargos
        public decimal AmountClp { get; set; }

        // Solo Transbank: recargo pasado al cliente (comisión Transbank + IVA sobre esa comisión)
        public decimal TransbankFee { get; set; }
        public decimal Iva { get; set; }

        // Lo que efectivamente paga el cliente (AmountClp + TransbankFee + Iva, o solo AmountClp si es transferencia)
        public decimal TotalClp { get; set; }

        // Tipo de cambio USDT/CLP congelado al crear el depósito
        public decimal RateSnapshot { get; set; }
        public decimal ExpectedUsdt { get; set; }
        public DateTime RateExpiresAt { get; set; }

        // Transbank
        public string? TransbankToken { get; set; }
        public string? BuyOrder { get; set; }
        public string? AuthorizationCode { get; set; }

        // Transferencia bancaria: comprobante subido por el usuario
        public string? ReceiptUrl { get; set; }

        // Revisión manual (transferencia) o auditoría (transbank)
        public Guid? ReviewedByAdminId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? AdminNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
