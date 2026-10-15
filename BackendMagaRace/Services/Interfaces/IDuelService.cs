using BackendMagaRace.Dtos.Duel;
using BackendMagaRace.Models;

namespace BackendMagaRace.Services.Interfaces
{
    public interface IDuelService
    {
        Task<Duel> CreateAndLockAsync(Guid creatorUserId, CreateDuelDto dto);

        Task<Duel> SettleAsync(Guid duelId, Guid reportingUserId, List<DuelResultDto> results);
    }
}
