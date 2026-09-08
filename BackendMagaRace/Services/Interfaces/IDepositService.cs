using BackendMagaRace.Models;
using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Services.Interfaces
{
    public interface IDepositService
    {
        Task<Deposit> CreateBankTransferDepositAsync(Guid userId, decimal amountClp);
        Task<Deposit> GetOwnedAsync(Guid userId, Guid depositId);
        Task<Deposit> AttachReceiptAsync(Guid userId, Guid depositId, string receiptUrl);
        Task<List<Deposit>> GetUserDepositsAsync(Guid userId);

        // Transbank
        Task<Deposit> CreateTransbankDepositAsync(Guid userId, decimal amountClp, TransbankPaymentMethod method, string returnUrl);
        Task<Deposit?> GetForRedirectAsync(Guid depositId);
        Task<Deposit> CommitTransbankDepositAsync(string token);
        Task<Deposit> MarkTransbankAbortedAsync(string token);

        // Admin
        Task<List<Deposit>> GetPendingReviewAsync();
        Task ApproveAsync(Guid depositId, Guid adminId, string? notes);
        Task RejectAsync(Guid depositId, Guid adminId, string reason);
    }
}
