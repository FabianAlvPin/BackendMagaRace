using BackendMagaRace.Data;
using BackendMagaRace.Dtos.Duel;
using BackendMagaRace.Models;
using BackendMagaRace.Models.Enums;
using BackendMagaRace.Options;
using BackendMagaRace.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackendMagaRace.Services
{
    public class DuelService : IDuelService
    {
        private readonly AppDbContext _db;
        private readonly IWalletService _wallet;
        private readonly DuelOptions _options;

        public DuelService(AppDbContext db, IWalletService wallet, IOptions<DuelOptions> options)
        {
            _db = db;
            _wallet = wallet;
            _options = options.Value;
        }

        public async Task<Duel> CreateAndLockAsync(Guid creatorUserId, CreateDuelDto dto)
        {
            var participantIds = dto.ParticipantUserIds.Distinct().ToList();

            if (participantIds.Count < 2 || participantIds.Count > _options.MaxParticipants)
                throw new InvalidOperationException(
                    $"Un duelo debe tener entre 2 y {_options.MaxParticipants} participantes.");

            if (!participantIds.Contains(creatorUserId))
                throw new InvalidOperationException("El creador del desafío debe estar en la lista de participantes.");

            // El monto debe ser 0 (sin apuesta) o calzar exacto con una de las 3 apuestas configuradas.
            bool esApuestaValida = dto.WagerUsdt == 0m
                || dto.WagerUsdt == _options.LowWagerUsdt
                || dto.WagerUsdt == _options.MidWagerUsdt
                || dto.WagerUsdt == _options.HighWagerUsdt;

            if (!esApuestaValida)
                throw new InvalidOperationException("El monto de la apuesta no es uno de los valores permitidos.");

            var ownsTransaction = _db.Database.CurrentTransaction == null;
            var transaction = _db.Database.CurrentTransaction
                ?? await _db.Database.BeginTransactionAsync();

            try
            {
                var duel = new Duel
                {
                    Id = Guid.NewGuid(),
                    CreatorUserId = creatorUserId,
                    WagerUsdt = dto.WagerUsdt,
                    RoomName = dto.RoomName,
                    Status = DuelStatus.Locked,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var userId in participantIds)
                {
                    // Si no hay apuesta, no se cobra nada: el duelo es solo competitivo.
                    if (dto.WagerUsdt > 0m)
                    {
                        await _wallet.SubtractCreditsAsync(
                            userId, dto.WagerUsdt, LedgerType.DuelWager, $"Duelo {duel.Id}");
                    }

                    duel.Participants.Add(new DuelParticipant
                    {
                        Id = Guid.NewGuid(),
                        DuelId = duel.Id,
                        UserId = userId
                    });
                }

                _db.Duels.Add(duel);

                await _db.SaveChangesAsync();
                if (ownsTransaction) await transaction.CommitAsync();

                return duel;
            }
            catch
            {
                if (ownsTransaction) await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (ownsTransaction) await transaction.DisposeAsync();
            }
        }

        public async Task<Duel> SettleAsync(Guid duelId, Guid reportingUserId, List<DuelResultDto> results)
        {
            var duel = await _db.Duels
                .Include(d => d.Participants)
                .FirstOrDefaultAsync(d => d.Id == duelId);

            if (duel == null)
                throw new KeyNotFoundException("Duelo no encontrado.");

            // Idempotente: si ya se liquidó (ej. un segundo reporte tras un cambio de
            // Master Client), se devuelve el resultado ya guardado sin volver a pagar.
            if (duel.Status == DuelStatus.Finished)
                return duel;

            if (duel.Status != DuelStatus.Locked)
                throw new InvalidOperationException("Este duelo no está en condiciones de liquidarse.");

            // No evita un cliente modificado (el backend no tiene forma de saber quién
            // es realmente el Master Client de Photon), pero sí bloquea que un usuario
            // completamente ajeno al duelo pueda liquidarlo.
            if (!duel.Participants.Any(p => p.UserId == reportingUserId))
                throw new InvalidOperationException("Quien reporta el resultado debe ser parte del duelo.");

            var participantIds = duel.Participants.Select(p => p.UserId).ToHashSet();
            var reportedIds = results.Select(r => r.UserId).ToHashSet();

            if (!participantIds.SetEquals(reportedIds))
                throw new InvalidOperationException(
                    "Los resultados reportados no coinciden con los participantes del duelo.");

            var ownsTransaction = _db.Database.CurrentTransaction == null;
            var transaction = _db.Database.CurrentTransaction
                ?? await _db.Database.BeginTransactionAsync();

            try
            {
                foreach (var participant in duel.Participants)
                {
                    var resultado = results.First(r => r.UserId == participant.UserId);
                    participant.FinalPosition = resultado.Disconnected ? null : resultado.FinalPosition;
                    participant.Disconnected = resultado.Disconnected;
                }

                if (duel.WagerUsdt > 0m)
                {
                    // Quien se desconecta pierde: su apuesta queda en el pozo, no se
                    // reembolsa, y no puede ganar ninguno de los lugares premiados.
                    var finishers = duel.Participants
                        .Where(p => !p.Disconnected && p.FinalPosition.HasValue)
                        .OrderBy(p => p.FinalPosition!.Value)
                        .ToList();

                    // La cantidad de lugares premiados depende del TOTAL de participantes
                    // del duelo (fijo desde que se creó), no de cuántos terminan la carrera.
                    decimal[] splitTable = duel.Participants.Count switch
                    {
                        2 => new[] { 1.00m },
                        3 => new[] { 0.70m, 0.30m },
                        _ => new[] { 0.50m, 0.30m, 0.20m }
                    };

                    var totalPot = duel.WagerUsdt * duel.Participants.Count;
                    var netPot = totalPot - (totalPot * _options.FeeRate);

                    for (int i = 0; i < splitTable.Length && i < finishers.Count; i++)
                    {
                        var premio = Math.Round(netPot * splitTable[i], 8);
                        finishers[i].PrizeUsdt = premio;

                        await _wallet.AddCreditsAsync(
                            finishers[i].UserId, premio, LedgerType.DuelPrize, $"Duelo {duel.Id}");
                    }
                }

                duel.Status = DuelStatus.Finished;
                duel.FinishedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                if (ownsTransaction) await transaction.CommitAsync();

                return duel;
            }
            catch
            {
                if (ownsTransaction) await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (ownsTransaction) await transaction.DisposeAsync();
            }
        }
    }
}
