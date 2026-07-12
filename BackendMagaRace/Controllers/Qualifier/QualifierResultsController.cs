using BackendMagaRace.Services;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace BackendMagaRace.Controllers.Qualifier
{
    [ApiController]
    [Route("api/qualifiers")]
    public class QualifierResultsController : ControllerBase
    {

        private readonly IQualifierService _service;


        public QualifierResultsController(
            IQualifierService service)
        {
            _service = service;
        }



        [HttpGet("{id}/results")]
        public async Task<IActionResult> GetResults(
            Guid id)
        {

            var result =
                await _service.GetResults(id);


            return Ok(result);
        }
    }
}