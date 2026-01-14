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
                Laps = dto.Laps,
                Status = RaceStatus.Waiting
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
        public async Task StartRace(Guid raceId)
        {
            var race = await _context.OnlineRaces
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.Id == raceId);

            if (race == null)
                throw new Exception("Carrera no existe");

            if (race.Status != RaceStatus.Waiting)
                throw new Exception("La carrera ya inició");

            if (race.Players.Count < 2)
                throw new Exception("No hay suficientes jugadores");

            race.Status = RaceStatus.InProgress;
            race.StartedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

    }
}
