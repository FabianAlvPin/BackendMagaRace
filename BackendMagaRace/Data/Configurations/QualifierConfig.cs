using BackendMagaRace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendMagaRace.Data.Configurations
{
    public class QualifierConfig : IEntityTypeConfiguration<QualifierEvent>
    {
        public void Configure(EntityTypeBuilder<QualifierEvent> e)
        {
            e.HasKey(x => x.Id);
            e.HasMany(x => x.Sessions)
             .WithOne(x => x.Event)
             .HasForeignKey(x => x.QualifierEventId);
        }
    }
}