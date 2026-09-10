using System;
using System.Collections.Generic;

namespace BackendMagaRace.Dtos.Wallet
{
    public class WalletMovementDto
    {
        public string Id { get; set; } = string.Empty;

        // "Deposito" | "Retiro" | "Premio"
        public string Kind { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Positivo si suma saldo, negativo si resta
        public decimal AmountUsdt { get; set; }

        public string StatusText { get; set; } = string.Empty;

        // "Green" | "Yellow" | "Red"
        public string StatusColor { get; set; } = string.Empty;

        // Fecha/hora del último cambio de estado
        public DateTime Date { get; set; }
    }

    public class WalletMovementsPageDto
    {
        public List<WalletMovementDto> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
