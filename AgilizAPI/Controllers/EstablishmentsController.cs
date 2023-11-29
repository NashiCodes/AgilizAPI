#region

using AgilizAPI.Models;
using AgilizAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Controllers;

[Route("/[controller]")]
[ApiController]
public class EstablishmentsController(IEstabRepo repo) : ControllerBase
{
    // GET: /Establishments
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<IEnumerable<EstablishmentDto>>> GetEstablishment()
    {
        return await repo.GetAll();
    }

    // GET: /Establishments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EstablishmentDto>> GetEstablishment(Guid id)
    {
        return await repo.GetEstabServices(id);
    }

    // PUT: /Establishments/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEstablishment(Guid id, Establishment establishment)
    {
        return await repo.EditarEstab(id, establishment);
    }

    // POST: /Establishments
    [HttpPost]
    public async Task<IActionResult> PostEstablishment(Establishment establishment)
    {
        return await repo.CadastrarEstab(establishment);
    }
}