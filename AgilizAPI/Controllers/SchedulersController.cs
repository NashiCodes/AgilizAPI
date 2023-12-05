#region

using AgilizAPI.Data;
using AgilizAPI.Models;
using AgilizAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AgilizAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SchedulersController(AgilizApiContext context, SchedulerRepo repo) : ControllerBase
{
    // GET: api/Schedulers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Scheduler>> GetScheduler(int id)
    {
        var scheduler = await context.Scheduler.FindAsync(id);

        if (scheduler == null) return NotFound();

        return scheduler;
    }

    // PUT: api/Schedulers/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult> PutScheduler(Guid id, Scheduler scheduler)
    {
        return (ActionResult)await repo.EditScheduler(id, scheduler);
    }

    // POST: api/Schedulers
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Scheduler>> PostScheduler(Scheduler scheduler)
    {
        await repo.CreateScheduler(scheduler);
        return CreatedAtAction("GetScheduler", new { id = scheduler.Id }, scheduler);
    }

    // DELETE: api/Schedulers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScheduler(Guid id)
    {
        return await repo.DeleteScheduler(id);
    }
}