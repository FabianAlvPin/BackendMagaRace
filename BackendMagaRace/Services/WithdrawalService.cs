using BackendMagaRace.Data;
using BackendMagaRace.Models;
using BackendMagaRace.Models.Enums;
using BackendMagaRace.Options;
using BackendMagaRace.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackendMagaRace.Services
{
    public class WithdrawalService : IWithdrawalService
    {
        private readonly AppDbContext _db;
        private readonly IExchangeRateService _fx;
        private readonly IWalletService _wallet;
        private readonly IWithdrawalAccountService _accounts;
        private readonly WithdrawalOptions _options;

        public WithdrawalService(
            AppDbContext db,
            IExchangeRateService fx,
            IWalletService wallet,
            IWithdrawalAccountService accounts,
            IOptions<WithdrawalOptions> options)
        {
            _db = db;
            _fx = fx;
            _wallet = wallet;
            _accounts = accounts;
            _options = options.Value;
        }

        public async Task<Withdrawal> CreateAsync(Guid userId, decimal amountUsdt)
        {
            if (amountUsdt <= 0)
                throw new InvalidOperationException("El monto debe ser mayor que 0");

            var account = await _accounts.GetAsync(userId);
            if (account == null)
                throw new InvalidOperationException("Debes configurar una cuenta de retiro antes de solicitar un retiro");

            var yaTienePendiente = await _db.Withdrawals
                .AnyAsync(w => w.UserId == userId && w.Status == WithdrawalStatus.Pending);
            if (yaTienePendiente)
                throw new InvalidOperationException("Ya tienes una solicitud de retiro pendiente. Espera a que se resuelva antes de solicitar otra.");

            var rate = await _fx.GetUsdtClpRateAsync();

            // La comisión reduce lo que el usuario recibe, no lo que se retiene de su wallet:
            // se descuenta AmountUsdt completo, pero el CLP pagado se calcula sobre el neto.
            var feeUsdt = Math.Round(amountUsdt * _options.FeeRate, 8, MidpointRounding.AwayFromZero);
            var netUsdt = amountUsdt - feeUsdt;
            var clpEquivalent = Math.Round(netUsdt * rate.Sell, 0, MidpointRounding.AwayFromZero);

            var withdrawal = new Withdrawal
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status = WithdrawalStatus.Pending,
                AmountUsdt = amountUsdt,
                FeeUsdt = feeUsdt,
                NetUsdt = netUsdt,
                RateSnapshot = rate.Sell,
                ClpEquivalent = clpEquivalent,
                Bank = account.Bank,
                AccountType = account.AccountType,
                AccountNumber = account.AccountNumber,
                Rut = account.Rut,
                HolderName = account.HolderName,
                CreatedAt = DateTime.UtcNow
            };

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Retiene el saldo de inmediato para que no se pueda gastar mientras se revisa
                await _wallet.SubtractCreditsAsync(userId, amountUsdt, LedgerType.WithdrawRequest, withdrawal.Id.ToString());

                _db.Withdrawals.Add(withdrawal);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException(ex.Message);
            }

            return withdrawal;
        }

        public async Task<Withdrawal> GetOwnedAsync(Guid userId, Guid withdrawalId)
        {
            var withdrawal = await _db.Withdrawals.FirstOrDefaultAsync(w => w.Id == withdrawalId && w.UserId == userId);
            if (withdrawal == null)
                throw new KeyNotFoundException("Retiro no encontrado");

            return withdrawal;
        }

        public async Task<List<Withdrawal>> GetUserWithdrawalsAsync(Guid userId)
        {
            return await _db.Withdrawals
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Withdrawal>> GetPendingAsync()
        {
            return await _db.Withdrawals
                .Include(w => w.User)
                .Where(w => w.Status == WithdrawalStatus.Pending)
                .OrderBy(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task ApproveAsync(Guid withdrawalId, Guid adminId, string? notes)
        {
            var withdrawal = await _db.Withdrawals.FirstOrDefaultAsync(w => w.Id == withdrawalId);
            if (withdrawal == null)
                throw new KeyNotFoundException("Retiro no encontrado");

            if (withdrawal.Status != WithdrawalStatus.Pending)
                throw new InvalidOperationException("Este retiro ya fue procesado");

            // El saldo ya se retuvo al solicitar el retiro, solo se actualiza el estado
            withdrawal.Status = WithdrawalStatus.Approved;
            withdrawal.ReviewedByAdminId = adminId;
            withdrawal.ReviewedAt = DateTime.UtcNow;
            withdrawal.AdminNotes = notes;

            await _db.SaveChangesAsync();
        }

        public async Task RejectAsync(Guid withdrawalId, Guid adminId, string reason)
        {
            var withdrawal = await _db.Withdrawals.FirstOrDefaultAsync(w => w.Id == withdrawalId);
            if (withdrawal == null)
                throw new KeyNotFoundException("Retiro no encontrado");

            if (withdrawal.Status != WithdrawalStatus.Pending)
                throw new InvalidOperationException("Este retiro ya fue procesado");

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Devuelve el saldo retenido
                await _wallet.AddCreditsAsync(withdrawal.UserId, withdrawal.AmountUsdt, LedgerType.WithdrawRejected, withdrawal.Id.ToString());

                withdrawal.Status = WithdrawalStatus.Rejected;
                withdrawal.ReviewedByAdminId = adminId;
                withdrawal.ReviewedAt = DateTime.UtcNow;
                withdrawal.AdminNotes = reason;

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
