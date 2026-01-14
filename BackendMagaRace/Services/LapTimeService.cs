using BackendMagaRace.Data;
using BackendMagaRace.Models;

namespace BackendMagaRace.Services
{
 

    public class LapTimeService
    {
        private readonly AppDbContext _context;

        public LapTimeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddLap(Guid userId, Guid trackId, int timeMs)
        {
            _context.LapTimes.Add(new LapTime
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TrackId = trackId,
                TimeMs = timeMs,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
    }

}
