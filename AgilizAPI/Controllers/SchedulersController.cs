#region

using AgilizAPI.Models;
using AgilizAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SchedulersController(SchedulerRepo repo) : ControllerBase
{
    // GET: api/Schedulers/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetScheduler(Guid id)
    {
        return await repo.GetScheduler(id).ConfigureAwait(false) as ActionResult ??
               new BadRequestObjectResult("Agendamento não encontrado");
    }


    // PUT: api/Schedulers/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult> PutScheduler(Guid id, Scheduler scheduler)
    {
        return await repo.EditScheduler(id, scheduler).ConfigureAwait(false) as ActionResult ??
               new BadRequestObjectResult("Agendamento não encontrado");
    }

    // POST: api/Schedulers
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult> PostScheduler(Scheduler scheduler)
    {
        return await repo.CreateScheduler(scheduler).ConfigureAwait(false) as ActionResult ??
               new BadRequestObjectResult("Agendamento não encontrado");
    }

    // DELETE: api/Schedulers/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteScheduler(Guid id)
    {
        return await repo.DeleteScheduler(id).ConfigureAwait(false) as ActionResult ??
               new BadRequestObjectResult("Agendamento não encontrado");
    }
}