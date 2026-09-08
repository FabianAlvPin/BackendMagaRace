using BackendMagaRace.Data;
using BackendMagaRace.Models;
using BackendMagaRace.Models.Enums;
using Microsoft.EntityFrameworkCore;
using BackendMagaRace.Services.Interfaces;
namespace BackendMagaRace.Services
{
    public class WalletService : IWalletService
    {
        private readonly AppDbContext _db;

        public WalletService(AppDbContext db)
        {
            _db = db;
        }

        // Obtener Wallet por usuario
        public async Task<Wallet> GetWalletAsync(Guid userId)
        {
            var wallet = await _db.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                throw new Exception("Wallet no encontrada");

            return wallet;
        }

        // Agregar créditos (ej: compra o premio)
        // Si ya hay una transacción en curso en este DbContext (ej: un caller que
        // combina esto con otro cambio de estado), se reutiliza en vez de anidar una nueva.
        public async Task AddCreditsAsync(Guid userId, decimal amount, LedgerType type, string reference)
        {
            if (amount <= 0) throw new Exception("El monto debe ser positivo");

            var ownsTransaction = _db.Database.CurrentTransaction == null;
            var transaction = _db.Database.CurrentTransaction
                ?? await _db.Database.BeginTransactionAsync();
            try
            {
                var wallet = await GetWalletAsync(userId);
                wallet.Balance += amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                _db.LedgerEntries.Add(new LedgerEntry
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = type,
                    Amount = amount,
                    Reference = reference,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
                if (ownsTransaction) await transaction.CommitAsync();
            }
            catch
            {
                if (ownsTransaction) await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (ownsTransaction) await transaction.DisposeAsync();
            }
        }

        // Restar créditos (ej: gasto en el juego)
        public async Task SubtractCreditsAsync(Guid userId, decimal amount, LedgerType type, string reference)
        {
            if (amount <= 0) throw new Exception("El monto debe ser positivo");

            var ownsTransaction = _db.Database.CurrentTransaction == null;
            var transaction = _db.Database.CurrentTransaction
                ?? await _db.Database.BeginTransactionAsync();
            try
            {
                var wallet = await GetWalletAsync(userId);
                if (wallet.Balance < amount)
                    throw new Exception("Saldo insuficiente");

                wallet.Balance -= amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                _db.LedgerEntries.Add(new LedgerEntry
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = type,
                    Amount = amount,
                    Reference = reference,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
                if (ownsTransaction) await transaction.CommitAsync();
            }
            catch
            {
                if (ownsTransaction) await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (ownsTransaction) await transaction.DisposeAsync();
            }
        }

        // Opcional: historial de movimientos
        public async Task<List<LedgerEntry>> GetLedgerAsync(Guid userId, int take = 50)
        {
            return await _db.LedgerEntries
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .Take(take)
                .ToListAsync();
        }
    }
}
