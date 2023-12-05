#region

using AgilizAPI.Models;
using AgilizAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Controllers;

[Route("/[controller]")]
[ApiController]
[Authorize]
public class EstablishmentsController(EstabRepo repo) : ControllerBase
{
    // GET: /Establishments
    [HttpGet("all/{local}")]
    [Authorize(Policy = "User")]
    public async Task<ActionResult<IEnumerable<EstablishmentDto>>> GetEstablishment([FromQuery] string local)
    {
        return await repo.GetAll(local);
    }

    // GET: /Establishments/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEstablishment([FromQuery] Guid id)
    {
        return await repo.GetEstabServices(id);
    }

    // PUT: /Establishments/5
    [HttpPut("{id}")]
    [Authorize(Policy = "Entrepreneur")]
    public async Task<IActionResult> PutEstablishment([FromQuery] Guid id, [FromBody] Establishment establishment)
    {
        return await repo.EditarEstab(id, establishment);
    }

    // POST: /Establishments
    [HttpPost]
    [Authorize(Policy = "Entrepreneur")]
    public async Task<IActionResult> PostEstablishment([FromBody] Establishment establishment)
    {
        return await repo.CadastrarEstab(establishment);
    }
}