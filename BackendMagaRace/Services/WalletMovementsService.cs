using BackendMagaRace.Data;
using BackendMagaRace.Dtos.Wallet;
using BackendMagaRace.Models;
using BackendMagaRace.Models.Enums;
using BackendMagaRace.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendMagaRace.Services
{
    public class WalletMovementsService : IWalletMovementsService
    {
        // Tipos de ledger que se muestran como "Premio" (no confundir con Purchase/WithdrawRequest/
        // WithdrawRejected, que ya se representan a través de Deposit/Withdrawal y no deben duplicarse aquí)
        private static readonly LedgerType[] PrizeTypes =
        {
            LedgerType.RaceWin,
            LedgerType.TournamentPrize,
            LedgerType.EventPrize,
            LedgerType.AdminCredit,
            LedgerType.AdminDebit
        };

        private readonly AppDbContext _db;

        public WalletMovementsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<WalletMovementsPageDto> GetMovementsAsync(Guid userId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var skip = (page - 1) * pageSize;

            // Para poder ordenar y paginar correctamente el feed combinado, se trae de cada
            // fuente hasta "skip + pageSize" filas (suficiente para cubrir cualquier página
            // aunque toda esa página venga de una sola fuente).
            var fetchLimit = skip + pageSize;

            var totalDeposits = await _db.Deposits.CountAsync(d => d.UserId == userId);
            var totalWithdrawals = await _db.Withdrawals.CountAsync(w => w.UserId == userId);
            var totalPrizes = await _db.LedgerEntries.CountAsync(l => l.UserId == userId && PrizeTypes.Contains(l.Type));

            var deposits = await _db.Deposits
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.ReviewedAt ?? d.CreatedAt)
                .Take(fetchLimit)
                .ToListAsync();

            var withdrawals = await _db.Withdrawals
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.ReviewedAt ?? w.CreatedAt)
                .Take(fetchLimit)
                .ToListAsync();

            var prizes = await _db.LedgerEntries
                .Where(l => l.UserId == userId && PrizeTypes.Contains(l.Type))
                .OrderByDescending(l => l.CreatedAt)
                .Take(fetchLimit)
                .ToListAsync();

            var movements = new List<WalletMovementDto>(deposits.Count + withdrawals.Count + prizes.Count);
            movements.AddRange(deposits.Select(MapDeposit));
            movements.AddRange(withdrawals.Select(MapWithdrawal));
            movements.AddRange(prizes.Select(MapPrize));

            var items = movements
                .OrderByDescending(m => m.Date)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return new WalletMovementsPageDto
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalDeposits + totalWithdrawals + totalPrizes
            };
        }

        private static WalletMovementDto MapDeposit(Deposit d)
        {
            var (statusText, color) = d.Status switch
            {
                DepositStatus.Pending => ("Pendiente de pago", "Yellow"),
                DepositStatus.PendingReview => ("En revisión", "Yellow"),
                DepositStatus.Completed => ("Completado", "Green"),
                DepositStatus.Rejected => ("Rechazado", "Red"),
                DepositStatus.Expired => ("Expirado", "Red"),
                _ => ("Desconocido", "Yellow")
            };

            var description = d.Method == DepositMethod.Transbank
                ? "Depósito Transbank"
                : "Depósito transferencia";

            return new WalletMovementDto
            {
                Id = d.Id.ToString(),
                Kind = "Deposito",
                Description = description,
                AmountUsdt = d.ExpectedUsdt,
                StatusText = statusText,
                StatusColor = color,
                Date = d.ReviewedAt ?? d.CreatedAt
            };
        }

        private static WalletMovementDto MapWithdrawal(Withdrawal w)
        {
            var (statusText, color) = w.Status switch
            {
                WithdrawalStatus.Pending => ("Pendiente", "Yellow"),
                WithdrawalStatus.Approved => ("Pagado", "Green"),
                WithdrawalStatus.Rejected => ("Rechazado", "Red"),
                _ => ("Desconocido", "Yellow")
            };

            return new WalletMovementDto
            {
                Id = w.Id.ToString(),
                Kind = "Retiro",
                Description = "Retiro",
                AmountUsdt = -w.AmountUsdt,
                StatusText = statusText,
                StatusColor = color,
                Date = w.ReviewedAt ?? w.CreatedAt
            };
        }

        private static WalletMovementDto MapPrize(LedgerEntry l)
        {
            var description = l.Type switch
            {
                LedgerType.RaceWin => "Premio carrera",
                LedgerType.TournamentPrize => "Premio torneo",
                LedgerType.EventPrize => "Premio evento",
                LedgerType.AdminCredit => "Ajuste admin",
                LedgerType.AdminDebit => "Ajuste admin",
                _ => "Premio"
            };

            // LedgerEntry.Amount siempre se guarda positivo; el signo lo da el Type
            var amount = l.Type == LedgerType.AdminDebit ? -l.Amount : l.Amount;

            return new WalletMovementDto
            {
                Id = l.Id.ToString(),
                Kind = "Premio",
                Description = description,
                AmountUsdt = amount,
                StatusText = "Completado",
                StatusColor = "Green",
                Date = l.CreatedAt
            };
        }
    }
}
