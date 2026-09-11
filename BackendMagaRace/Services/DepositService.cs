using BackendMagaRace.Data;
using BackendMagaRace.Models;
using BackendMagaRace.Models.Enums;
using BackendMagaRace.Options;
using BackendMagaRace.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackendMagaRace.Services
{
    public class DepositService : IDepositService
    {
        private static readonly TimeSpan QuoteValidity = TimeSpan.FromMinutes(15);

        private readonly AppDbContext _db;
        private readonly IExchangeRateService _fx;
        private readonly IWalletService _wallet;
        private readonly ITransbankService _transbank;
        private readonly TransbankOptions _transbankOptions;

        public DepositService(
            AppDbContext db,
            IExchangeRateService fx,
            IWalletService wallet,
            ITransbankService transbank,
            IOptions<TransbankOptions> transbankOptions)
        {
            _db = db;
            _fx = fx;
            _wallet = wallet;
            _transbank = transbank;
            _transbankOptions = transbankOptions.Value;
        }

        public async Task<Deposit> CreateBankTransferDepositAsync(Guid userId, decimal amountClp)
        {
            if (amountClp <= 0)
                throw new InvalidOperationException("El monto debe ser mayor que 0");

            // Solo se permite una transferencia bancaria activa a la vez: si el usuario
            // cierra la app después de generarla, al volver no hay ambigüedad sobre a
            // cuál depósito corresponde el comprobante que suba.
            var pendiente = await _db.Deposits.FirstOrDefaultAsync(d =>
                d.UserId == userId &&
                d.Method == DepositMethod.BankTransfer &&
                d.Status == DepositStatus.PendingReview);

            if (pendiente != null)
            {
                if (DateTime.UtcNow > pendiente.RateExpiresAt)
                {
                    // La cotización ya venció sin que se subiera comprobante: se marca
                    // expirada automáticamente en vez de dejar bloqueado al usuario.
                    pendiente.Status = DepositStatus.Expired;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException(
                        "Ya tienes una transferencia pendiente. Sube el comprobante o espera a que se resuelva antes de generar otra.");
                }
            }

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
                // Toma el lock de la wallet del usuario antes de re-chequear el estado: si
                // dos aprobaciones casi simultáneas llegan aquí (doble clic del admin), la
                // segunda queda esperando a que la primera termine, y recién ahí ve que el
                // depósito ya fue procesado, en vez de acreditar el saldo dos veces.
                await _wallet.AddCreditsAsync(deposit.UserId, deposit.ExpectedUsdt, LedgerType.Purchase, deposit.Id.ToString());

                await _db.Entry(deposit).ReloadAsync();
                if (deposit.Status != DepositStatus.PendingReview)
                    throw new InvalidOperationException("Este depósito ya fue procesado");

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

        // ======================================================
        // TRANSBANK
        // ======================================================

        public async Task<Deposit> CreateTransbankDepositAsync(Guid userId, decimal amountClp, TransbankPaymentMethod method, string returnUrl)
        {
            if (amountClp <= 0)
                throw new InvalidOperationException("El monto debe ser mayor que 0");

            var rate = await _fx.GetUsdtClpRateAsync();

            var feeRate = method == TransbankPaymentMethod.Credit
                ? _transbankOptions.CreditFeeRate
                : _transbankOptions.DebitFeeRate;

            var fee = Math.Round(amountClp * feeRate, 0, MidpointRounding.AwayFromZero);
            var iva = Math.Round(fee * _transbankOptions.IvaRate, 0, MidpointRounding.AwayFromZero);
            var totalClp = amountClp + fee + iva;

            var deposit = new Deposit
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Method = DepositMethod.Transbank,
                Status = DepositStatus.Pending,
                AmountClp = amountClp,
                TransbankFee = fee,
                Iva = iva,
                TotalClp = totalClp,
                RateSnapshot = rate.Buy,
                ExpectedUsdt = Math.Round(amountClp / rate.Buy, 8),
                RateExpiresAt = DateTime.UtcNow.Add(QuoteValidity),
                CreatedAt = DateTime.UtcNow
            };

            // Transbank exige buyOrder <= 26 caracteres
            var buyOrder = "D" + deposit.Id.ToString("N")[..20];
            var sessionId = userId.ToString("N");

            var (token, formUrl) = _transbank.CreateTransaction(buyOrder, sessionId, totalClp, returnUrl);

            deposit.BuyOrder = buyOrder;
            deposit.TransbankToken = token;
            deposit.TransbankFormUrl = formUrl;

            _db.Deposits.Add(deposit);
            await _db.SaveChangesAsync();
            return deposit;
        }

        public async Task<Deposit?> GetForRedirectAsync(Guid depositId)
        {
            return await _db.Deposits.FirstOrDefaultAsync(d => d.Id == depositId);
        }

        private async Task<Deposit> GetByTransbankTokenAsync(string token)
        {
            var deposit = await _db.Deposits.FirstOrDefaultAsync(d => d.TransbankToken == token);
            if (deposit == null)
                throw new KeyNotFoundException("Depósito no encontrado");

            return deposit;
        }

        public async Task<Deposit> CommitTransbankDepositAsync(string token)
        {
            var deposit = await GetByTransbankTokenAsync(token);

            // Idempotencia: Transbank puede reintentar el POST de retorno
            if (deposit.Status != DepositStatus.Pending)
                return deposit;

            var result = _transbank.CommitTransaction(token);

            if (result.IsAuthorized)
            {
                await using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    // Mismo patrón que en ApproveAsync: se toma el lock de la wallet primero,
                    // y recién con el lock tomado se re-chequea el estado. Protege contra un
                    // reintento duplicado del POST de retorno de Transbank llegando casi al
                    // mismo tiempo (no acredita el mismo depósito dos veces).
                    await _wallet.AddCreditsAsync(deposit.UserId, deposit.ExpectedUsdt, LedgerType.Purchase, deposit.Id.ToString());

                    await _db.Entry(deposit).ReloadAsync();
                    if (deposit.Status != DepositStatus.Pending)
                    {
                        await transaction.RollbackAsync();
                        return deposit;
                    }

                    deposit.AuthorizationCode = result.AuthorizationCode;
                    deposit.CardTypeCode = result.PaymentTypeCode;
                    deposit.Status = DepositStatus.Completed;
                    deposit.ReviewedAt = DateTime.UtcNow;

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            else
            {
                deposit.AuthorizationCode = result.AuthorizationCode;
                deposit.CardTypeCode = result.PaymentTypeCode;
                deposit.Status = DepositStatus.Rejected;
                deposit.AdminNotes = $"Transbank rechazó el pago (status={result.Status}, responseCode={result.ResponseCode})";
                deposit.ReviewedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return deposit;
        }

        public async Task<Deposit> MarkTransbankAbortedAsync(string token)
        {
            var deposit = await GetByTransbankTokenAsync(token);

            if (deposit.Status == DepositStatus.Pending)
            {
                deposit.Status = DepositStatus.Rejected;
                deposit.AdminNotes = "El usuario canceló el pago en Transbank";
                deposit.ReviewedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return deposit;
        }
    }
}
