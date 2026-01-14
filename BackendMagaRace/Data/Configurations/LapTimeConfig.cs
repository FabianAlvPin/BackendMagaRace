using BackendMagaRace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendMagaRace.Data.Configurations
{
    public class LapTimeConfig : IEntityTypeConfiguration<LapTime>
    {
        public void Configure(EntityTypeBuilder<LapTime> e)
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.TrackId });
            e.Property(x => x.TimeMs).IsRequired();
        }
    }
}
