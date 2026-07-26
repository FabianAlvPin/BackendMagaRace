using BackendMagaRace.Data;
using BackendMagaRace.Dtos.OnlineRace;
using BackendMagaRace.Dtos.Qualifier;
using BackendMagaRace.Models;
using BackendMagaRace.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendMagaRace.Services
{
    public class QualifierService : IQualifierService
    {
        private readonly AppDbContext _context;


        public QualifierService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<QualifierSession?> GetActiveSession(
    Guid userId,
    Guid eventId)
        {
            var session = await _context.QualifierSessions
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.QualifierEventId == eventId &&
                    x.ActiveUntil > DateTime.UtcNow
                );

            return session;
        }
        public class BusinessException : Exception
        {
            public string Code { get; }

            public BusinessException(string code, string message)
                : base(message)
            {
                Code = code;
            }
        }
        public async Task<QualifierSession> Join(Guid userId, Guid qualifierEventId)
        {
            // 1. Validar si ya tiene sesión activa
            var existingSession = await _context.QualifierSessions
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.QualifierEventId == qualifierEventId &&
                    x.ActiveUntil > DateTime.UtcNow);

            if (existingSession != null)
                return existingSession;

            // 2. Validar evento
            var qualifierEvent = await _context.QualifierEvents
                .FirstOrDefaultAsync(x =>
                    x.Id == qualifierEventId &&
                    !x.IsClosed);

            if (qualifierEvent == null)
                throw new BusinessException("EVENT_NOT_AVAILABLE", "El evento no existe o ya finalizó.");

            // 3. Verificar si ya compró entrada
            var existingEntry = await _context.QualifierEntries
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.QualifierEventId == qualifierEventId);

            // 4. Si no tiene entrada, cobrar
            if (existingEntry == null)
            {
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (wallet == null)
                    throw new BusinessException("WALLET_NOT_FOUND", "No se encontró la billetera del jugador.");

                if (wallet.Balance < qualifierEvent.EntryCost)
                    throw new BusinessException("INSUFFICIENT_BALANCE",
                        $"Saldo insuficiente. Saldo: ${wallet.Balance:N0} - Entrada: ${qualifierEvent.EntryCost:N0}");

                wallet.Balance -= qualifierEvent.EntryCost;

                _context.QualifierEntries.Add(new QualifierEntry
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    QualifierEventId = qualifierEventId,
                    EntryCost = qualifierEvent.EntryCost,
                    PurchasedAt = DateTime.UtcNow,
                    ActiveUntil = qualifierEvent.EndsAt
                });
            }

            // 5. Crear sesión
            var session = new QualifierSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                QualifierEventId = qualifierEventId,
                ActiveUntil = qualifierEvent.EndsAt,
                BestLapMs = null
            };

            _context.QualifierSessions.Add(session);

            await _context.SaveChangesAsync();

            return session;
        }
        public async Task<object> GetActiveEvents()
        {
            var now = DateTime.UtcNow;

            var events = await _context.QualifierEvents
                .Where(x =>
                    !x.IsClosed &&
                    x.StartsAt <= now &&
                    x.EndsAt >= now)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.TrackId,
                    TrackName = x.Track.Name,
                    x.StartsAt,
                    x.EndsAt,
                    x.Direction,
                    x.Transmission,
                    x.CarCategory,
                    x.Laps,
                    x.EntryCost,
                    x.BasePrize,
                    CurrentPrizePool = x.BasePrize + (x.Entries.Count * x.EntryCost),
                    Prizes = x.Prizes.Select(p => new
                    {
                        p.Id,
                        p.FromPosition,
                        p.ToPosition,
                        p.PrizePercent
                    }).ToList()
                })
                .ToListAsync();

            return events;
        }



        public async Task<object?> GetEvent(Guid eventId)
        {
            return await _context.QualifierEvents
                .Where(x => x.Id == eventId)
                .Select(x => new
                {
                    x.Id,
                    x.TrackId,
                    x.StartsAt,
                    x.EndsAt,
                    x.EntryCost,
                   
                    x.IsClosed,
                    CurrentPrizePool = x.BasePrize + (x.Entries.Count * x.EntryCost)
                })
                .FirstOrDefaultAsync();
        }



        public async Task<object> StartSession(
            Guid userId,
            Guid qualifierEventId)
        {

            var session =
                await _context.QualifierSessions
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.QualifierEventId == qualifierEventId &&
                    x.ActiveUntil > DateTime.UtcNow);



            if (session != null)
                return session;



            var ev =
                await _context.QualifierEvents
                .FirstOrDefaultAsync(x =>
                    x.Id == qualifierEventId);



            if (ev == null)
                throw new Exception("Evento no existe");



            var newSession = new QualifierSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                QualifierEventId = qualifierEventId,
                CreatedAt = DateTime.UtcNow,
                ActiveUntil = ev.EndsAt,
                BestLapMs = null
            };


            _context.QualifierSessions.Add(newSession);

            await _context.SaveChangesAsync();


            return newSession;
        }



        public async Task<object> SubmitLap(
    Guid userId,
    Guid sessionId,
    int timeMs)
        {
            var session =
                await _context.QualifierSessions
                .FirstOrDefaultAsync(x =>
                    x.Id == sessionId &&
                    x.UserId == userId);


            if (session == null)
                throw new Exception("Sesión inválida");


            if (session.ActiveUntil < DateTime.UtcNow)
                throw new Exception("Sesión expirada");


            if (session.BestLapMs == null ||
                timeMs < session.BestLapMs)
            {
                session.BestLapMs = timeMs;

                await _context.SaveChangesAsync();
            }


            return new
            {
                bestLapMs = session.BestLapMs
            };
        }
        public async Task<object?> GetPlayerPosition(
    Guid eventId,
    Guid userId)
        {
            var ranking = await _context.QualifierSessions
                .Where(x =>
                    x.QualifierEventId == eventId &&
                    x.BestLapMs != null)
                .OrderBy(x => x.BestLapMs)
                .Select(x => new
                {
                    x.UserId,
                    x.BestLapMs
                })
                .ToListAsync();


            var position = ranking
                .FindIndex(x => x.UserId == userId);


            if (position == -1)
            {
                return null;
            }


            return new
            {
                Position = position + 1,
                BestLapMs = ranking[position].BestLapMs,
                TotalPlayers = ranking.Count
            };
        }

        public async Task<object?> GetSession(Guid sessionId)
        {
            return await _context.QualifierSessions
                .FirstOrDefaultAsync(x =>
                    x.Id == sessionId);
        }



        public async Task<object> GetRanking(Guid eventId)
        {
            var ranking = await _context.QualifierSessions
                .Where(x =>
                    x.QualifierEventId == eventId &&
                    x.BestLapMs != null)
                .OrderBy(x => x.BestLapMs)
                .Select(x => new
                {
                    Position = 0,                 // se reemplaza más abajo
                    x.UserId,
                    Username = x.User.Username,
                    BestLapMs = x.BestLapMs.Value
                })
                .ToListAsync();

            return ranking
                .Select((x, index) => new
                {
                    Position = index + 1,
                    x.UserId,
                    x.Username,
                    x.BestLapMs
                })
                .ToList();
        }



        public async Task<object> GetResults(Guid eventId)
        {
            return await GetRanking(eventId);
        }



        public async Task CloseEvent(Guid eventId)
        {
            var ev =
                await _context.QualifierEvents
                .FirstOrDefaultAsync(x =>
                    x.Id == eventId);


            if (ev == null)
                throw new Exception("Evento no encontrado");


            ev.IsClosed = true;

            await _context.SaveChangesAsync();
        }



        public async Task<QualifierEvent> CreateEvent(CreateQualifierEventDto dto)
        {
            var qualifierEvent = new QualifierEvent
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                TrackId = dto.TrackId,
                StartsAt = dto.StartsAt,
                EndsAt = dto.EndsAt,
                EntryCost = dto.EntryCost,
                BasePrize = dto.BasePrize,
                IsClosed = false,
                Direction = dto.Direction,
                CarCategory = dto.CarCategory,
                Transmission = dto.Transmission,
                Laps = dto.Laps,
            };

            _context.QualifierEvents.Add(qualifierEvent);

            await _context.SaveChangesAsync();

            return qualifierEvent;
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