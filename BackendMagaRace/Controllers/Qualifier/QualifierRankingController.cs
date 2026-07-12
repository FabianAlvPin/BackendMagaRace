using BackendMagaRace.Services;
using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


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
    }
}