using System.Security.Claims;
using BackendMagaRace.Dtos.Duel;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMagaRace.Controllers
{
    [ApiController]
    [Route("duels")]
    [Authorize]
    public class DuelController : ControllerBase
    {
        private readonly IDuelService _duels;

        public DuelController(IDuelService duels)
        {
            _duels = duels;
        }

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new Exception("UserId no encontrado en token");
            return Guid.Parse(userIdClaim.Value);
        }

        // POST /duels/create-and-lock
        // Llamado por el desafiante en Free World una vez se cierra la ventana de
        // aceptación, con la lista final de quienes van a correr.
        [HttpPost("create-and-lock")]
        public async Task<IActionResult> CreateAndLock([FromBody] CreateDuelDto dto)
        {
            var userId = GetUserIdFromToken();

            try
            {
                var duel = await _duels.CreateAndLockAsync(userId, dto);
                return Ok(new
                {
                    id = duel.Id,
                    roomName = duel.RoomName,
                    wagerUsdt = duel.WagerUsdt,
                    participantUserIds = duel.Participants.Select(p => p.UserId)
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // WalletService.SubtractCreditsAsync lanza System.Exception genérica
                // (ej. "Saldo insuficiente"), no InvalidOperationException — sin este
                // catch, un jugador sin saldo suficiente tiraba un 500 sin cuerpo.
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST /duels/{id}/settle
        // Llamado por el Master Client de la sala de Photon al terminar la carrera,
        // con el ranking final de todos los participantes.
        [HttpPost("{id}/settle")]
        public async Task<IActionResult> Settle(Guid id, [FromBody] SettleDuelDto dto)
        {
            var userId = GetUserIdFromToken();

            try
            {
                var duel = await _duels.SettleAsync(id, userId, dto.Results);
                return Ok(new
                {
                    id = duel.Id,
                    status = duel.Status.ToString(),
                    participants = duel.Participants.Select(p => new
                    {
                        userId = p.UserId,
                        finalPosition = p.FinalPosition,
                        disconnected = p.Disconnected,
                        prizeUsdt = p.PrizeUsdt
                    })
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
