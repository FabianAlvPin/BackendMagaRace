using BackendMagaRace.Models;

namespace BackendMagaRace.Services.Interfaces
{
    public interface IWithdrawalAccountService
    {
        Task<WithdrawalAccount?> GetAsync(Guid userId);
        Task<WithdrawalAccount> SetAsync(Guid userId, string bank, string accountType, string accountNumber, string rut, string holderName);
    }
}
