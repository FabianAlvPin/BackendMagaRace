using System;

namespace BackendMagaRace.Services.Interfaces
{
    // Buy = precio para comprar USDT con CLP (ask) -> usar en depósitos
    // Sell = precio para vender USDT por CLP (bid) -> usar en retiros
    public record UsdtClpRate(decimal Buy, decimal Sell, DateTime FetchedAt);

    public interface IExchangeRateService
    {
        Task<UsdtClpRate> GetUsdtClpRateAsync();
    }
}
