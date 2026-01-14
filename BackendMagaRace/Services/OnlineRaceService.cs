using BackendMagaRace.Data;
using BackendMagaRace.Dtos.OnlineRace;
using BackendMagaRace.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendMagaRace.Services
{
    public class OnlineRaceService
    {
        private readonly AppDbContext _context;

        public OnlineRaceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OnlineRace> Create(CreateOnlineRaceDto dto)
        {
            var race = new OnlineRace
            {
                Id = Guid.NewGuid(),
                TrackId = dto.TrackId,
                RoomName = dto.RoomName,
                EntryCost = dto.EntryCost,
                MaxPlayers = dto.MaxPlayers,
                Laps = dto.Laps
            };

            _context.OnlineRaces.Add(race);
            await _context.SaveChangesAsync();
            return race;
        }

        public async Task Join(Guid raceId, Guid userId)
        {
            _context.OnlineRacePlayers.Add(new OnlineRacePlayer
            {
                Id = Guid.NewGuid(),
                OnlineRaceId = raceId,
                UserId = userId
            });

            await _context.SaveChangesAsync();
        }
    }
}
