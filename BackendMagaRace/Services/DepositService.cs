using BackendMagaRace.Data;
using BackendMagaRace.Models;
using BackendMagaRace.Models.Enums;
using BackendMagaRace.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendMagaRace.Services
{
    public class DepositService : IDepositService
    {
        private static readonly TimeSpan QuoteValidity = TimeSpan.FromMinutes(15);

        private readonly AppDbContext _db;
        private readonly IExchangeRateService _fx;
        private readonly IWalletService _wallet;

        public DepositService(AppDbContext db, IExchangeRateService fx, IWalletService wallet)
        {
            _db = db;
            _fx = fx;
            _wallet = wallet;
        }

        public async Task<Deposit> CreateBankTransferDepositAsync(Guid userId, decimal amountClp)
        {
            if (amountClp <= 0)
                throw new InvalidOperationException("El monto debe ser mayor que 0");

            var rate = await _fx.GetUsdtClpRateAsync();

            var deposit = new Deposit
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Method = DepositMethod.BankTransfer,
                Status = DepositStatus.PendingReview,
                AmountClp = amountClp,
                TransbankFee = 0,
                Iva = 0,
                TotalClp = amountClp,
                RateSnapshot = rate.Buy,
                ExpectedUsdt = Math.Round(amountClp / rate.Buy, 8),
                RateExpiresAt = DateTime.UtcNow.Add(QuoteValidity),
                CreatedAt = DateTime.UtcNow
            };

            _db.Deposits.Add(deposit);
            await _db.SaveChangesAsync();
            return deposit;
        }

        public async Task<Deposit> GetOwnedAsync(Guid userId, Guid depositId)
        {
            var deposit = await _db.Deposits
                .FirstOrDefaultAsync(d => d.Id == depositId && d.UserId == userId);

            if (deposit == null)
                throw new KeyNotFoundException("Depósito no encontrado");

            return deposit;
        }

        public async Task<Deposit> AttachReceiptAsync(Guid userId, Guid depositId, string receiptUrl)
        {
            var deposit = await GetOwnedAsync(userId, depositId);

            if (deposit.Status != DepositStatus.PendingReview)
                throw new InvalidOperationException("Este depósito ya fue procesado");

            if (DateTime.UtcNow > deposit.RateExpiresAt)
            {
                deposit.Status = DepositStatus.Expired;
                await _db.SaveChangesAsync();
                throw new InvalidOperationException("La cotización expiró, crea un nuevo depósito");
            }

            deposit.ReceiptUrl = receiptUrl;
            await _db.SaveChangesAsync();
            return deposit;
        }

        public async Task<List<Deposit>> GetUserDepositsAsync(Guid userId)
        {
            return await _db.Deposits
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Deposit>> GetPendingReviewAsync()
        {
            return await _db.Deposits
                .Include(d => d.User)
                .Where(d => d.Status == DepositStatus.PendingReview && d.ReceiptUrl != null)
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task ApproveAsync(Guid depositId, Guid adminId, string? notes)
        {
            var deposit = await _db.Deposits.FirstOrDefaultAsync(d => d.Id == depositId);
            if (deposit == null)
                throw new KeyNotFoundException("Depósito no encontrado");

            if (deposit.Status != DepositStatus.PendingReview)
                throw new InvalidOperationException("Este depósito ya fue procesado");

            if (string.IsNullOrEmpty(deposit.ReceiptUrl))
                throw new InvalidOperationException("El depósito no tiene comprobante adjunto");

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                await _wallet.AddCreditsAsync(deposit.UserId, deposit.ExpectedUsdt, LedgerType.Purchase, deposit.Id.ToString());

                deposit.Status = DepositStatus.Completed;
                deposit.ReviewedByAdminId = adminId;
                deposit.ReviewedAt = DateTime.UtcNow;
                deposit.AdminNotes = notes;

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RejectAsync(Guid depositId, Guid adminId, string reason)
        {
            var deposit = await _db.Deposits.FirstOrDefaultAsync(d => d.Id == depositId);
            if (deposit == null)
                throw new KeyNotFoundException("Depósito no encontrado");

            if (deposit.Status != DepositStatus.PendingReview)
                throw new InvalidOperationException("Este depósito ya fue procesado");

            deposit.Status = DepositStatus.Rejected;
            deposit.ReviewedByAdminId = adminId;
            deposit.ReviewedAt = DateTime.UtcNow;
            deposit.AdminNotes = reason;

            await _db.SaveChangesAsync();
        }
    }
}
