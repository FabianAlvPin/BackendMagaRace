using BackendMagaRace.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendMagaRace.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User → Wallet (1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId);

            // Wallet → Ledger (1:N)
            modelBuilder.Entity<LedgerEntry>()
                .HasIndex(x => x.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
