using BackendMagaRace.Models;
using BackendMagaRace.Models.Enums;
using BackendMagaRace.Services;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
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

        // POST /wallet/add -> agregar créditos
        [HttpPost("add")]
        public async Task<IActionResult> AddCredits([FromBody] WalletOperationDto dto)
        {
            if (dto.Amount <= 0) return BadRequest("Amount debe ser mayor que 0");

            var userId = GetUserIdFromToken();
            await _walletService.AddCreditsAsync(userId, dto.Amount, dto.Type, dto.Reference);
            return Ok(new { message = "Créditos agregados" });
        }

        // POST /wallet/subtract -> gastar créditos
        [HttpPost("subtract")]
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
