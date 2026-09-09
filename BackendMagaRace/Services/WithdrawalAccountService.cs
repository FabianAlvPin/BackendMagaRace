using BackendMagaRace.Data;
using BackendMagaRace.Models;
using BackendMagaRace.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendMagaRace.Services
{
    public class WithdrawalAccountService : IWithdrawalAccountService
    {
        private readonly AppDbContext _db;

        public WithdrawalAccountService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<WithdrawalAccount?> GetAsync(Guid userId)
        {
            return await _db.WithdrawalAccounts.FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<WithdrawalAccount> SetAsync(Guid userId, string bank, string accountType, string accountNumber, string rut, string holderName)
        {
            var account = await _db.WithdrawalAccounts.FirstOrDefaultAsync(w => w.UserId == userId);

            if (account == null)
            {
                account = new WithdrawalAccount
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _db.WithdrawalAccounts.Add(account);
            }

            account.Bank = bank;
            account.AccountType = accountType;
            account.AccountNumber = accountNumber;
            account.Rut = rut;
            account.HolderName = holderName;
            account.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return account;
        }
    }
}
