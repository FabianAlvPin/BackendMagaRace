using BackendMagaRace.Dtos.Wallet;

namespace BackendMagaRace.Services.Interfaces
{
    public interface IWalletMovementsService
    {
        Task<WalletMovementsPageDto> GetMovementsAsync(Guid userId, int page, int pageSize);
    }
}
