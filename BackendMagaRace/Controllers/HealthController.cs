using Microsoft.AspNetCore.Mvc;

namespace BackendMagaRace.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "ok",
                service = "BackendMagaRace",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
