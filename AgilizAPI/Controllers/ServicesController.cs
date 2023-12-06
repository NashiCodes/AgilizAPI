#region

using AgilizAPI.Models;
using AgilizAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Controllers;

[Route("a|[controller]")]
[ApiController]
public class ServicesController(ServicesRepo repo) : ControllerBase
{
    // GET: api/Services/only/
    [HttpGet("/only/{id}")]
    public async Task<IActionResult> GetServices(Guid id)
    {
        return await repo.GetServiceOnly(id).ConfigureAwait(false);
    }

    // GET: api/Services/5
    [HttpGet("{estabId}")]
    public async Task<IActionResult> GetServicesEstb(Guid estabId)
    {
        return await repo.GetServicesEstab(estabId).ConfigureAwait(false);
    }

    // PUT: api/Services/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutServices(Guid id, Service service)
    {
        if (id != service.Id) return BadRequest();

        return await repo.EditService(id, service).ConfigureAwait(false);
    }

    // POST: api/Services
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<IActionResult> PostServices(Service service)
    {
        return await repo.CreateService(service).ConfigureAwait(false);
    }

    // DELETE: api/Services/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServices(Guid id)
    {
        return await repo.DeleteService(id).ConfigureAwait(false);
    }
}