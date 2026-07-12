using BackendMagaRace.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendMagaRace.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }


        // ===== EXISTENTES =====

        public DbSet<User> Users => Set<User>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();


        // ===== NUEVOS =====

        public DbSet<LapTime> LapTimes => Set<LapTime>();

        public DbSet<QualifierEvent> QualifierEvents => Set<QualifierEvent>();

        public DbSet<QualifierSession> QualifierSessions => Set<QualifierSession>();

        public DbSet<QualifierPrize> QualifierPrizes => Set<QualifierPrize>();

        public DbSet<OnlineRace> OnlineRaces => Set<OnlineRace>();

        public DbSet<OnlineRacePlayer> OnlineRacePlayers => Set<OnlineRacePlayer>();

        public DbSet<QualifierEntry> QualifierEntries => Set<QualifierEntry>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ======================
            // USER
            // ======================

            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId);


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();



            // ======================
            // WALLET / LEDGER
            // ======================

            modelBuilder.Entity<LedgerEntry>()
                .HasIndex(x => x.UserId);



            // ======================
            // LAP TIMES
            // ======================

            modelBuilder.Entity<LapTime>(e =>
            {
                e.HasKey(x => x.Id);


                e.HasIndex(x => new { x.UserId, x.TrackId });


                e.Property(x => x.TimeMs)
                    .IsRequired();


                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });



            // ======================
            // QUALIFIER EVENT
            // ======================

            modelBuilder.Entity<QualifierEvent>(e =>
            {
                e.HasKey(x => x.Id);


                e.HasMany(x => x.Sessions)
                    .WithOne(x => x.Event)
                    .HasForeignKey(x => x.QualifierEventId)
                    .OnDelete(DeleteBehavior.Cascade);


                // NUEVO: Premios del evento

                e.HasMany(x => x.Prizes)
                    .WithOne(x => x.Event)
                    .HasForeignKey(x => x.QualifierEventId)
                    .OnDelete(DeleteBehavior.Cascade);

            });



            // ======================
            // QUALIFIER SESSION
            // ======================

            modelBuilder.Entity<QualifierSession>(e =>
            {
                e.HasKey(x => x.Id);


                e.HasIndex(x => new { x.UserId, x.QualifierEventId });


                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });



            // ======================
            // QUALIFIER PRIZE
            // ======================

            modelBuilder.Entity<QualifierPrize>(e =>
            {
                e.HasKey(x => x.Id);


                e.HasIndex(x => x.QualifierEventId);


                e.Property(x => x.FixedAmount)
                    .HasPrecision(18, 2);


                e.Property(x => x.PrizePercent)
                    .HasPrecision(5, 2);

            });



            // ======================
            // ONLINE RACE
            // ======================

            modelBuilder.Entity<OnlineRace>(e =>
            {
                e.HasKey(x => x.Id);


                e.Property(x => x.Status)
                    .HasConversion<int>()
                    .IsRequired();


                e.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("now()");


                e.HasMany(x => x.Players)
                    .WithOne(x => x.OnlineRace)
                    .HasForeignKey(x => x.OnlineRaceId)
                    .OnDelete(DeleteBehavior.Cascade);

            });



            modelBuilder.Entity<OnlineRacePlayer>(e =>
            {
                e.HasKey(x => x.Id);


                e.HasIndex(x => new { x.OnlineRaceId, x.UserId });


                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.HasKey(x => x.Id);

                e.HasIndex(x => x.Token)
                    .IsUnique();

                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.Token)
                    .IsRequired();

                e.Property(x => x.ExpiresAt)
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}