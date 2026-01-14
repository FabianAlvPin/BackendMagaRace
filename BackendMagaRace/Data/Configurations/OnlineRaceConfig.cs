using BackendMagaRace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendMagaRace.Data.Configurations
{
    public class OnlineRaceConfig : IEntityTypeConfiguration<OnlineRace>
    {
        public void Configure(EntityTypeBuilder<OnlineRace> e)
        {
            e.HasKey(x => x.Id);
            e.HasMany(x => x.Players)
             .WithOne(x => x.OnlineRace)
             .HasForeignKey(x => x.OnlineRaceId);
        }
    }
}
