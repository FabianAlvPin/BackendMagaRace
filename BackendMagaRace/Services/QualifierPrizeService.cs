using BackendMagaRace.Data;
using BackendMagaRace.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BackendMagaRace.Models;
using BackendMagaRace.Dtos.Qualifier;

namespace BackendMagaRace.Services
{
    public class QualifierPrizeService : IQualifierPrizeService
    {

        private readonly AppDbContext _context;


        public QualifierPrizeService(AppDbContext context)
        {
            _context = context;
        }



        public async Task<decimal> CalculatePrizePool(Guid qualifierEventId)
        {
            var ev = await _context.QualifierEvents
                .FirstAsync(x => x.Id == qualifierEventId);

            var totalEntries = await _context.QualifierEntries
                .CountAsync(x => x.QualifierEventId == qualifierEventId);

            return ev.BasePrize + (totalEntries * ev.EntryCost);
        }



        public async Task CalculatePrizes(
            Guid qualifierEventId)
        {
            // pendiente:
            // calcular posiciones
            // aplicar QualifierPrize
        }



        public async Task DistributePrizes(
            Guid qualifierEventId)
        {
            // pendiente:
            // crear LedgerEntries
            // acreditar Wallets
        }



        public async Task<object> GetPrizeResults(
            Guid qualifierEventId)
        {

            var prizes =
                await _context.QualifierPrizes
                .Where(x =>
                    x.QualifierEventId == qualifierEventId)
                .ToListAsync();


            return prizes;
        }
        public async Task<QualifierPrize> Create(CreateQualifierPrizeDto dto)
        {
            var prize = new QualifierPrize
            {
                Id = Guid.NewGuid(),
                QualifierEventId = dto.QualifierEventId,
                FromPosition = dto.FromPosition,
                ToPosition = dto.ToPosition,
                PrizePercent = dto.PrizePercent
            };

            _context.QualifierPrizes.Add(prize);

            await _context.SaveChangesAsync();

            return prize;
        }
    }
}