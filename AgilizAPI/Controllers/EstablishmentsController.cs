#region

using AgilizAPI.Models;
using AgilizAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Controllers;

[Route("/[controller]")]
[ApiController]
public class EstablishmentsController(EstabRepo repo) : ControllerBase
{
    // GET: /Establishments/all/
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<EstabDtoRaw>>> GetEstablishment([FromQuery] string local)
    {
        return await repo.GetAll(local).ConfigureAwait(false);

        //retorna todos os estabelecimentos de uma cidade como uma lista de EstabDtoRaw
    }

    // GET: /Establishments/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEstablishment([FromQuery] Guid id)
    {
        return await repo.GetEstabServices(id).ConfigureAwait(false);

        //retorna todos os serviços de um estabelecimento como uma lista de ServiceDto
    }

    // PUT: /Establishments/5
    [HttpPut("{id}")]
    [Authorize(Policy = "Entrepreneur")]
    public async Task<IActionResult> PutEstablishment([FromQuery] Guid id, [FromBody] Establishment establishment)
    {
        return await repo.EditarEstab(id, establishment).ConfigureAwait(false);

        //edita um estabelecimento
    }

    // POST: /Establishments
    [HttpPost]
    [Authorize(Policy = "Entrepreneur")]
    public async Task<IActionResult> PostEstablishment([FromBody] EstabRegister establishment)
    {
        return await repo.CadastrarEstab(establishment).ConfigureAwait(false);

        //cadastra um estabelecimento
    }
}