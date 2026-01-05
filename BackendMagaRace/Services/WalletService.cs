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
        public async Task AddCreditsAsync(Guid userId, decimal amount, LedgerType type, string reference)
        {
            if (amount <= 0) throw new Exception("El monto debe ser positivo");

            using var transaction = await _db.Database.BeginTransactionAsync();
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
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Restar créditos (ej: gasto en el juego)
        public async Task SubtractCreditsAsync(Guid userId, decimal amount, LedgerType type, string reference)
        {
            if (amount <= 0) throw new Exception("El monto debe ser positivo");

            using var transaction = await _db.Database.BeginTransactionAsync();
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
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
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
