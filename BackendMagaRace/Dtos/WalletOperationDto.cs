using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Dtos
{
    public class WalletOperationDto
    {
        public decimal Amount { get; set; }

        // Tipo de operación, por defecto RaceWin
        public LedgerType Type { get; set; } = LedgerType.RaceWin;

        public string Reference { get; set; } = "";
    }
}
