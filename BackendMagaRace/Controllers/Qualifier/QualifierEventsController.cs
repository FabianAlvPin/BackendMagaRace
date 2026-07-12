using BackendMagaRace.Dtos.Qualifier;
using BackendMagaRace.Services;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMagaRace.Controllers.Qualifier
{
    [ApiController]
    [Route("api/qualifiers")]
    public class QualifierEventsController : ControllerBase
    {
        private readonly IQualifierService _qualifierService;

        public QualifierEventsController(
            IQualifierService qualifierService)
        {
            _qualifierService = qualifierService;
        }


        [HttpGet("active")]
        public async Task<IActionResult> GetActiveEvents()
        {
            var result = await _qualifierService.GetActiveEvents();

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var result = await _qualifierService.GetEvent(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
          [FromBody] CreateQualifierEventDto dto)
        {
            var result = await _qualifierService.CreateEvent(dto);

            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/close")]
        public async Task<IActionResult> CloseEvent(Guid id)
        {
            await _qualifierService.CloseEvent(id);

            return Ok(new
            {
                message = "Qualifier cerrado correctamente"
            });
        }
    }
}