using BackendMagaRace.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("qualifier/prize")]
public class QualifierPrizeController : ControllerBase
{
    private readonly IQualifierPrizeService _service;

    public QualifierPrizeController(IQualifierPrizeService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateQualifierPrizeDto dto)
    {
        return Ok(await _service.Create(dto));
    }
}