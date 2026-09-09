using BackendMagaRace.Dtos.Wallet;
using BackendMagaRace.Models;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendMagaRace.Controllers
{
    [ApiController]
    [Route("wallet/withdrawals")]
    [Authorize]
    public class WithdrawalsController : ControllerBase
    {
        private readonly IWithdrawalService _withdrawals;
        private readonly IWithdrawalAccountService _accounts;

        public WithdrawalsController(IWithdrawalService withdrawals, IWithdrawalAccountService accounts)
        {
            _withdrawals = withdrawals;
            _accounts = accounts;
        }

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new Exception("UserId no encontrado en token");
            return Guid.Parse(userIdClaim.Value);
        }

        // GET /wallet/withdrawals/account
        [HttpGet("account")]
        public async Task<IActionResult> GetAccount()
        {
            var userId = GetUserIdFromToken();
            var account = await _accounts.GetAsync(userId);
            if (account == null)
                return NotFound(new { error = "No has configurado una cuenta de retiro" });

            return Ok(ToAccountDto(account));
        }

        // PUT /wallet/withdrawals/account
        [HttpPut("account")]
        public async Task<IActionResult> SetAccount([FromBody] SetWithdrawalAccountDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Bank) || string.IsNullOrWhiteSpace(dto.AccountType) ||
                string.IsNullOrWhiteSpace(dto.AccountNumber) || string.IsNullOrWhiteSpace(dto.Rut) ||
                string.IsNullOrWhiteSpace(dto.HolderName))
            {
                return BadRequest(new { error = "Todos los campos son obligatorios" });
            }

            var userId = GetUserIdFromToken();
            var account = await _accounts.SetAsync(userId, dto.Bank, dto.AccountType, dto.AccountNumber, dto.Rut, dto.HolderName);
            return Ok(ToAccountDto(account));
        }

        // POST /wallet/withdrawals
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWithdrawalDto dto)
        {
            var userId = GetUserIdFromToken();
            try
            {
                var withdrawal = await _withdrawals.CreateAsync(userId, dto.AmountUsdt);
                return Ok(ToResponseDto(withdrawal));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET /wallet/withdrawals
        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetUserIdFromToken();
            var withdrawals = await _withdrawals.GetUserWithdrawalsAsync(userId);
            return Ok(withdrawals.Select(ToResponseDto));
        }

        // GET /wallet/withdrawals/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(Guid id)
        {
            var userId = GetUserIdFromToken();
            try
            {
                var withdrawal = await _withdrawals.GetOwnedAsync(userId, id);
                return Ok(ToResponseDto(withdrawal));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        // ======================================================
        // ADMIN
        // ======================================================

        // GET /wallet/withdrawals/admin/pending
        [HttpGet("admin/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPending()
        {
            var withdrawals = await _withdrawals.GetPendingAsync();
            return Ok(withdrawals.Select(w => new AdminWithdrawalDto
            {
                Id = w.Id,
                UserId = w.UserId,
                Username = w.User.Username,
                Status = w.Status,
                AmountUsdt = w.AmountUsdt,
                RateSnapshot = w.RateSnapshot,
                ClpEquivalent = w.ClpEquivalent,
                Bank = w.Bank,
                AccountType = w.AccountType,
                AccountNumber = w.AccountNumber,
                Rut = w.Rut,
                HolderName = w.HolderName,
                CreatedAt = w.CreatedAt
            }));
        }

        // POST /wallet/withdrawals/admin/{id}/approve
        [HttpPost("admin/{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveWithdrawalDto dto)
        {
            var adminId = GetUserIdFromToken();
            try
            {
                await _withdrawals.ApproveAsync(id, adminId, dto.Notes);
                return Ok(new { message = "Retiro aprobado" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST /wallet/withdrawals/admin/{id}/reject
        [HttpPost("admin/{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectWithdrawalDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest(new { error = "Debes indicar un motivo de rechazo" });

            var adminId = GetUserIdFromToken();
            try
            {
                await _withdrawals.RejectAsync(id, adminId, dto.Reason);
                return Ok(new { message = "Retiro rechazado, saldo devuelto" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private static WithdrawalAccountResponseDto ToAccountDto(WithdrawalAccount a) => new()
        {
            Bank = a.Bank,
            AccountType = a.AccountType,
            AccountNumber = a.AccountNumber,
            Rut = a.Rut,
            HolderName = a.HolderName,
            UpdatedAt = a.UpdatedAt
        };

        private static WithdrawalResponseDto ToResponseDto(Withdrawal w) => new()
        {
            Id = w.Id,
            Status = w.Status,
            AmountUsdt = w.AmountUsdt,
            RateSnapshot = w.RateSnapshot,
            ClpEquivalent = w.ClpEquivalent,
            Bank = w.Bank,
            AccountType = w.AccountType,
            AccountNumber = w.AccountNumber,
            Rut = w.Rut,
            HolderName = w.HolderName,
            CreatedAt = w.CreatedAt
        };
    }
}
