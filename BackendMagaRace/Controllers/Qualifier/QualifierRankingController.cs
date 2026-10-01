using BackendMagaRace.Services;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace BackendMagaRace.Controllers.Qualifier
{
    [ApiController]
    [Route("api/qualifiers")]
    public class QualifierRankingController : ControllerBase
    {

        private readonly IQualifierService _service;


        public QualifierRankingController(
            IQualifierService service)
        {
            _service = service;
        }



        [HttpGet("{id}/ranking")]
        public async Task<IActionResult> GetRanking(
            Guid id)
        {

            var ranking =
                await _service.GetRanking(id);


            return Ok(ranking);
        }

        // Versión liviana de la ranking para sondeo periódico durante la carrera:
        // solo trae el líder (top1) y mi propia posición/tiempo, sin el costo de
        // traer y ordenar el ranking completo del evento en cada llamada.
        [HttpGet("{id}/leaderboard-summary")]
        public async Task<IActionResult> GetLeaderboardSummary(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new { message = "El token no contiene el identificador del usuario." });

            var userId = Guid.Parse(userIdClaim.Value);

            var summary = await _service.GetLeaderboardSummary(id, userId);

            return Ok(summary);
        }
    }
}