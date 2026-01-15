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
                OwnerUserId = dto.OwnerUserId,
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

        public async Task JoinRaceAsync(Guid raceId, Guid userId)
        {
            var race = await _context.OnlineRaces
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.Id == raceId);

            if (race == null)
                throw new InvalidOperationException("La carrera no existe");

            if (race.Status != RaceStatus.Waiting)
                throw new InvalidOperationException("La carrera ya comenzó");

            if (race.Players.Count >= race.MaxPlayers)
                throw new InvalidOperationException("La carrera está llena");

            var alreadyJoined = race.Players
                .Any(p => p.UserId == userId);

            if (alreadyJoined)
                throw new InvalidOperationException("El jugador ya está inscrito");

            var player = new OnlineRacePlayer
            {
                Id = Guid.NewGuid(),
                OnlineRaceId = race.Id,
                UserId = userId,
                IsOwner = race.OwnerUserId == userId,
                JoinedAt = DateTime.UtcNow,
                IsConnected = true
            };

            _context.OnlineRacePlayers.Add(player);
            await _context.SaveChangesAsync();
        }

        public async Task<OnlineRace> StartRaceAsync(Guid raceId, Guid userId)
        {
            var race = await _context.OnlineRaces
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.Id == raceId);

            if (race == null)
                throw new InvalidOperationException("RACE_NOT_FOUND");

            // Solo el creador puede iniciar (opcional pero recomendado)
            var isOwner = race.Players.Any(p => p.UserId == userId && p.IsOwner);
            if (!isOwner)
                throw new InvalidOperationException("NOT_RACE_OWNER");

            // Idempotente
            if (race.Status == RaceStatus.InProgress)
                return race;

            if (race.Status != RaceStatus.Waiting)
                throw new InvalidOperationException("RACE_NOT_WAITING");

            if (race.Players.Count < 2)
                throw new InvalidOperationException("NOT_ENOUGH_PLAYERS");

            race.Status = RaceStatus.InProgress;
            race.StartedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return race;
        }


    }
}
