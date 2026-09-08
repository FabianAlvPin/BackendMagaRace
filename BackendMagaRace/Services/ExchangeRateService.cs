using System.Net.Http.Json;
using BackendMagaRace.Services.Interfaces;

namespace BackendMagaRace.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private const string ApiUrl = "https://criptoya.com/api/usdt/clp/1";
        private const string Exchange = "binancep2p";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(60);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private UsdtClpRate? _cache;

        public ExchangeRateService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<UsdtClpRate> GetUsdtClpRateAsync()
        {
            if (_cache != null && DateTime.UtcNow - _cache.FetchedAt < CacheDuration)
                return _cache;

            await _lock.WaitAsync();
            try
            {
                if (_cache != null && DateTime.UtcNow - _cache.FetchedAt < CacheDuration)
                    return _cache;

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetFromJsonAsync<Dictionary<string, CriptoYaQuote>>(ApiUrl);

                if (response == null || !response.TryGetValue(Exchange, out var quote))
                    throw new InvalidOperationException("No se pudo obtener el tipo de cambio USDT/CLP");

                _cache = new UsdtClpRate(quote.Ask, quote.Bid, DateTime.UtcNow);
                return _cache;
            }
            finally
            {
                _lock.Release();
            }
        }

        private class CriptoYaQuote
        {
            public decimal Ask { get; set; }
            public decimal Bid { get; set; }
        }
    }
}
