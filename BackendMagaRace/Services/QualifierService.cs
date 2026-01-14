using BackendMagaRace.Data;
using BackendMagaRace.Dtos.Qualifier;
using BackendMagaRace.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendMagaRace.Services
{
    public class QualifierService
    {
        private readonly AppDbContext _context;

        public QualifierService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<QualifierSession> Join(Guid userId, Guid eventId)
        {
            var session = new QualifierSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                QualifierEventId = eventId,
                ActiveUntil = DateTime.UtcNow.AddHours(10)
            };

            _context.QualifierSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<List<QualifierRankingDto>> GetRanking(Guid eventId)
        {
            return await _context.QualifierSessions
                .Include(x => x.User)
                .Where(x => x.QualifierEventId == eventId && x.BestLapMs != null)
                .OrderBy(x => x.BestLapMs)
                .Select(x => new QualifierRankingDto
                {
                    Username = x.User.Username,
                    BestLapMs = x.BestLapMs.Value
                })
                .ToListAsync();
        }
    }
}
