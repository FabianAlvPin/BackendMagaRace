using BackendMagaRace.Dtos.Wallet;
using BackendMagaRace.Models.Enums;
using BackendMagaRace.Options;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BackendMagaRace.Controllers
{
    [ApiController]
    [Route("wallet/deposits")]
    [Authorize]
    public class DepositsController : ControllerBase
    {
        private static readonly string[] AllowedContentTypes =
            { "image/jpeg", "image/png", "image/webp", "application/pdf" };

        private const long MaxReceiptBytes = 5 * 1024 * 1024; // 5 MB

        private readonly IDepositService _deposits;
        private readonly CompanyBankAccountOptions _bankAccount;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public DepositsController(
            IDepositService deposits,
            IOptions<CompanyBankAccountOptions> bankAccount,
            IWebHostEnvironment env,
            IConfiguration config)
        {
            _deposits = deposits;
            _bankAccount = bankAccount.Value;
            _env = env;
            _config = config;
        }

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new Exception("UserId no encontrado en token");
            return Guid.Parse(userIdClaim.Value);
        }

        // GET /wallet/deposits/bank-info
        [HttpGet("bank-info")]
        public IActionResult GetBankInfo()
        {
            return Ok(new BankInfoDto
            {
                Bank = _bankAccount.Bank,
                AccountType = _bankAccount.AccountType,
                AccountNumber = _bankAccount.AccountNumber,
                Rut = _bankAccount.Rut,
                HolderName = _bankAccount.HolderName,
                Email = _bankAccount.Email
            });
        }

        // POST /wallet/deposits/bank-transfer
        [HttpPost("bank-transfer")]
        public async Task<IActionResult> CreateBankTransfer([FromBody] CreateBankTransferDepositDto dto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var deposit = await _deposits.CreateBankTransferDepositAsync(userId, dto.AmountClp);
                return Ok(ToResponseDto(deposit));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST /wallet/deposits/{id}/receipt
        [HttpPost("{id}/receipt")]
        [RequestSizeLimit(MaxReceiptBytes)]
        public async Task<IActionResult> UploadReceipt(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Debes adjuntar un archivo" });

            if (file.Length > MaxReceiptBytes)
                return BadRequest(new { error = "El archivo supera el tamaño máximo (5MB)" });

            if (!AllowedContentTypes.Contains(file.ContentType))
                return BadRequest(new { error = "Formato no permitido (usa JPG, PNG, WEBP o PDF)" });

            var userId = GetUserIdFromToken();

            try
            {
                var deposit = await _deposits.GetOwnedAsync(userId, id);

                var uploadsDir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "receipts");
                Directory.CreateDirectory(uploadsDir);

                var ext = Path.GetExtension(file.FileName);
                var fileName = $"{deposit.Id}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);

                await using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                var receiptUrl = $"/uploads/receipts/{fileName}";
                var updated = await _deposits.AttachReceiptAsync(userId, id, receiptUrl);

                return Ok(ToResponseDto(updated));
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

        // GET /wallet/deposits
        [HttpGet]
        public async Task<IActionResult> GetMyDeposits()
        {
            var userId = GetUserIdFromToken();
            var deposits = await _deposits.GetUserDepositsAsync(userId);
            return Ok(deposits.Select(ToResponseDto));
        }

        // GET /wallet/deposits/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(Guid id)
        {
            var userId = GetUserIdFromToken();
            try
            {
                var deposit = await _deposits.GetOwnedAsync(userId, id);
                return Ok(ToResponseDto(deposit));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        // ======================================================
        // TRANSBANK
        // ======================================================

        // POST /wallet/deposits/transbank
        [HttpPost("transbank")]
        public async Task<IActionResult> CreateTransbank([FromBody] CreateTransbankDepositDto dto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var baseUrl = _config["PublicBaseUrl"]?.TrimEnd('/');
                var returnUrl = $"{baseUrl}/wallet/deposits/transbank/return";

                var deposit = await _deposits.CreateTransbankDepositAsync(userId, dto.AmountClp, dto.Method, returnUrl);
                var redirectUrl = $"{baseUrl}/wallet/deposits/transbank/{deposit.Id}/start";

                return Ok(new { deposit = ToResponseDto(deposit), redirectUrl });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET /wallet/deposits/transbank/{id}/start
        // Se abre en el navegador del dispositivo (sin JWT), por eso es anónimo.
        [HttpGet("transbank/{id}/start")]
        [AllowAnonymous]
        public async Task<IActionResult> StartTransbank(Guid id)
        {
            var deposit = await _deposits.GetForRedirectAsync(id);

            if (deposit == null
                || deposit.Method != DepositMethod.Transbank
                || deposit.Status != DepositStatus.Pending
                || string.IsNullOrEmpty(deposit.TransbankToken)
                || string.IsNullOrEmpty(deposit.TransbankFormUrl))
            {
                return NotFound("Depósito no válido o ya procesado");
            }

            var html = $@"<!DOCTYPE html>
<html><body onload=""document.forms[0].submit()"">
<form method=""POST"" action=""{deposit.TransbankFormUrl}"">
<input type=""hidden"" name=""token_ws"" value=""{deposit.TransbankToken}"" />
</form>
Redirigiendo a Webpay...
</body></html>";

            return Content(html, "text/html");
        }

        // POST /wallet/deposits/transbank/return
        // Transbank hace este POST (form-urlencoded) al terminar el pago o al cancelarlo.
        [HttpPost("transbank/return")]
        [AllowAnonymous]
        public async Task<IActionResult> TransbankReturn(
            [FromForm(Name = "token_ws")] string? tokenWs,
            [FromForm(Name = "TBK_TOKEN")] string? tbkToken)
        {
            string html;
            try
            {
                if (!string.IsNullOrEmpty(tokenWs))
                {
                    await _deposits.CommitTransbankDepositAsync(tokenWs);
                    html = "<html><body><h3>Pago procesado. Puedes volver a la aplicación.</h3></body></html>";
                }
                else if (!string.IsNullOrEmpty(tbkToken))
                {
                    await _deposits.MarkTransbankAbortedAsync(tbkToken);
                    html = "<html><body><h3>Pago cancelado. Puedes volver a la aplicación.</h3></body></html>";
                }
                else
                {
                    html = "<html><body><h3>Respuesta inválida de Transbank.</h3></body></html>";
                }
            }
            catch (Exception)
            {
                html = "<html><body><h3>Ocurrió un problema procesando el pago. Puedes volver a la aplicación.</h3></body></html>";
            }

            return Content(html, "text/html");
        }

        // ======================================================
        // ADMIN
        // ======================================================

        // GET /wallet/deposits/admin/pending
        [HttpGet("admin/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPending()
        {
            var deposits = await _deposits.GetPendingReviewAsync();
            return Ok(deposits.Select(d => new AdminDepositDto
            {
                Id = d.Id,
                UserId = d.UserId,
                Username = d.User.Username,
                Status = d.Status,
                AmountClp = d.AmountClp,
                TotalClp = d.TotalClp,
                ExpectedUsdt = d.ExpectedUsdt,
                RateSnapshot = d.RateSnapshot,
                RateExpiresAt = d.RateExpiresAt,
                ReceiptUrl = d.ReceiptUrl,
                CreatedAt = d.CreatedAt
            }));
        }

        // POST /wallet/deposits/admin/{id}/approve
        [HttpPost("admin/{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveDepositDto dto)
        {
            var adminId = GetUserIdFromToken();
            try
            {
                await _deposits.ApproveAsync(id, adminId, dto.Notes);
                return Ok(new { message = "Depósito aprobado y créditos asignados" });
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

        // POST /wallet/deposits/admin/{id}/reject
        [HttpPost("admin/{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectDepositDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest(new { error = "Debes indicar un motivo de rechazo" });

            var adminId = GetUserIdFromToken();
            try
            {
                await _deposits.RejectAsync(id, adminId, dto.Reason);
                return Ok(new { message = "Depósito rechazado" });
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

        private static DepositResponseDto ToResponseDto(Models.Deposit d) => new()
        {
            Id = d.Id,
            Method = d.Method,
            Status = d.Status,
            AmountClp = d.AmountClp,
            TransbankFee = d.TransbankFee,
            Iva = d.Iva,
            TotalClp = d.TotalClp,
            RateSnapshot = d.RateSnapshot,
            ExpectedUsdt = d.ExpectedUsdt,
            RateExpiresAt = d.RateExpiresAt,
            ReceiptUrl = d.ReceiptUrl,
            CreatedAt = d.CreatedAt
        };
    }
}
