using BackendMagaRace.Data;
using BackendMagaRace.Models;
using BackendMagaRace.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendMagaRace.Services
{
    public class QualifierEntryService : IQualifierEntryService
    {

        private readonly AppDbContext _context;


        public QualifierEntryService(AppDbContext context)
        {
            _context = context;
        }



        public async Task<object> BuyEntry(
            Guid userId,
            Guid qualifierEventId)
        {

            var existing =
                await _context.QualifierEntries
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.QualifierEventId == qualifierEventId &&
                    x.ActiveUntil > DateTime.UtcNow);



            if (existing != null)
                return existing;



            var ev =
                await _context.QualifierEvents
                .FirstAsync(x =>
                    x.Id == qualifierEventId);



            var entry = new QualifierEntry
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                QualifierEventId = qualifierEventId,
                EntryCost = ev.EntryCost,
                PurchasedAt = DateTime.UtcNow,
                ActiveUntil = ev.EndsAt
            };


            _context.QualifierEntries.Add(entry);

            await _context.SaveChangesAsync();


            return entry;
        }



        public async Task<object?> GetEntry(
            Guid userId,
            Guid qualifierEventId)
        {

            return await _context.QualifierEntries
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.QualifierEventId == qualifierEventId);
        }



        public async Task<bool> HasValidEntry(
            Guid userId,
            Guid qualifierEventId)
        {

            return await _context.QualifierEntries
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.QualifierEventId == qualifierEventId &&
                    x.ActiveUntil > DateTime.UtcNow);
        }
    }
}