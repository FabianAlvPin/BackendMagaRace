using BackendMagaRace.Models;

namespace BackendMagaRace.Services.Interfaces
{
    public interface IWithdrawalService
    {
        Task<Withdrawal> CreateAsync(Guid userId, decimal amountUsdt);
        Task<Withdrawal> GetOwnedAsync(Guid userId, Guid withdrawalId);
        Task<List<Withdrawal>> GetUserWithdrawalsAsync(Guid userId);

        // Admin
        Task<List<Withdrawal>> GetPendingAsync();
        Task ApproveAsync(Guid withdrawalId, Guid adminId, string? notes);
        Task RejectAsync(Guid withdrawalId, Guid adminId, string reason);
    }
}
