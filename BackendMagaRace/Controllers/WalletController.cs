using BackendMagaRace.Models;
using BackendMagaRace.Models.Enums;
using BackendMagaRace.Options;
using BackendMagaRace.Services;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using BackendMagaRace.Dtos;

namespace BackendMagaRace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize] // JWT obligatorio
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IWalletMovementsService _movementsService;
        private readonly TransbankOptions _transbankOptions;

        public WalletController(
            IWalletService walletService,
            IExchangeRateService exchangeRateService,
            IWalletMovementsService movementsService,
            IOptions<TransbankOptions> transbankOptions)
        {
            _walletService = walletService;
            _exchangeRateService = exchangeRateService;
            _movementsService = movementsService;
            _transbankOptions = transbankOptions.Value;
        }

        // Helper: obtiene UserId desde el token JWT
        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new Exception("UserId no encontrado en token");
            return Guid.Parse(userIdClaim.Value);
        }

        // GET /wallet -> saldo actual
        [HttpGet]
        public async Task<IActionResult> GetWallet()
        {
            var userId = GetUserIdFromToken();
            var wallet = await _walletService.GetWalletAsync(userId);
            return Ok(new { wallet.Balance });
        }

        // POST /wallet/add -> agregar créditos (uso interno/admin: los flujos de depósito
        // reales deben pasar por sus propios endpoints, no por este genérico)
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCredits([FromBody] WalletOperationDto dto)
        {
            if (dto.Amount <= 0) return BadRequest("Amount debe ser mayor que 0");

            var userId = GetUserIdFromToken();
            await _walletService.AddCreditsAsync(userId, dto.Amount, dto.Type, dto.Reference);
            return Ok(new { message = "Créditos agregados" });
        }

        // POST /wallet/subtract -> gastar créditos (uso interno/admin)
        [HttpPost("subtract")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SubtractCredits([FromBody] WalletOperationDto dto)
        {
            if (dto.Amount <= 0) return BadRequest("Amount debe ser mayor que 0");

            var userId = GetUserIdFromToken();
            try
            {
                await _walletService.SubtractCreditsAsync(userId, dto.Amount, dto.Type, dto.Reference);
                return Ok(new { message = "Créditos descontados" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET /wallet/rate -> tipo de cambio USDT/CLP vigente + tasas de comisión Transbank
        // (estas últimas se exponen para que el cliente pueda calcular el desglose de un
        // depósito Transbank en vivo, sin tener que crear el depósito para verlo)
        [HttpGet("rate")]
        public async Task<IActionResult> GetRate()
        {
            var rate = await _exchangeRateService.GetUsdtClpRateAsync();
            return Ok(new
            {
                rate.Buy,
                rate.Sell,
                rate.FetchedAt,
                _transbankOptions.CreditFeeRate,
                _transbankOptions.DebitFeeRate,
                _transbankOptions.IvaRate
            });
        }

        // GET /wallet/movements?page=1&pageSize=20 -> depositos + retiros + premios, unificados y paginados
        [HttpGet("movements")]
        public async Task<IActionResult> GetMovements([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetUserIdFromToken();
            var result = await _movementsService.GetMovementsAsync(userId, page, pageSize);
            return Ok(result);
        }

        // GET /wallet/ledger -> historial
        [HttpGet("ledger")]
        public async Task<IActionResult> GetLedger()
        {
            var userId = GetUserIdFromToken();
            var ledger = await _walletService.GetLedgerAsync(userId);
            return Ok(ledger);
        }
    }
}
