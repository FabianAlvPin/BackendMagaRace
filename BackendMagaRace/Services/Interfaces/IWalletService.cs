using BackendMagaRace.Models;
using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Services.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> GetWalletAsync(Guid userId);
        Task AddCreditsAsync(Guid userId, decimal amount, LedgerType type, string reference);
        Task SubtractCreditsAsync(Guid userId, decimal amount, LedgerType type, string reference);
        Task<List<LedgerEntry>> GetLedgerAsync(Guid userId, int take = 50);
    }
}
