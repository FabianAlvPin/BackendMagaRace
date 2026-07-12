using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendMagaRace.Services.Interfaces;

namespace BackendMagaRace.Controllers.Qualifier
{
    [ApiController]
    [Route("api/qualifiers")]
    [Authorize]
    public class QualifierEntriesController : ControllerBase
    {
        private readonly IQualifierEntryService _entryService;


        public QualifierEntriesController(
            IQualifierEntryService entryService)
        {
            _entryService = entryService;
        }


        [HttpPost("{id}/entry")]
        public async Task<IActionResult> BuyEntry(Guid id)
        {
            var userId = Guid.Parse(
                User.FindFirst("sub")!.Value
            );


            var result =
                await _entryService.BuyEntry(
                    userId,
                    id);


            return Ok(result);
        }


        [HttpGet("{id}/entry")]
        public async Task<IActionResult> GetEntry(Guid id)
        {
            var userId = Guid.Parse(
                User.FindFirst("sub")!.Value
            );


            var result =
                await _entryService.GetEntry(
                    userId,
                    id);


            return Ok(result);
        }
    }
}