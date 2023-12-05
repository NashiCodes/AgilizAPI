#region

using AgilizAPI.Data;
using AgilizAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AgilizAPI.Repositories;

public class SchedulerRepo(AgilizApiContext context)
{
    public async Task<IActionResult> GetScheduler(Guid id)
    {
        var scheduler = await context.Scheduler.FindAsync(id);

        return scheduler is null
                   ? new NotFoundObjectResult("Agendamento não encontrado")
                   : new OkObjectResult(scheduler);
    }

    public async Task<List<Scheduler>> GetSchedulerByService(Guid id)
    {
        return await context.Scheduler.Where(s => s.IdService == id).ToListAsync();
    }

    public async Task<List<Scheduler>> GetSchedulerByUser(string id)
    {
        return await context.Scheduler.Where(s => s.IdUser == id).ToListAsync();
    }

    public async Task<IActionResult> CreateScheduler(Scheduler scheduler)
    {
        try
        {
            await context.Scheduler.AddAsync(scheduler);
            await context.SaveChangesAsync();
            return new OkObjectResult(scheduler);
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    public async Task<IActionResult> EditScheduler(Guid id, Scheduler scheduler)
    {
        try
        {
            var schedulerDb = await context.Scheduler.FindAsync(id);
            if (schedulerDb is null) return new NotFoundObjectResult("Agendamento não encontrado");

            schedulerDb.Date       = scheduler.Date;
            schedulerDb.IdService  = scheduler.IdService;
            schedulerDb.IdUser     = scheduler.IdUser;
            schedulerDb.currStatus = scheduler.currStatus;

            context.Scheduler.Update(schedulerDb);
            await context.SaveChangesAsync();
            return new OkObjectResult(schedulerDb);
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    public async Task<IActionResult> DeleteScheduler(Guid id)
    {
        try
        {
            var scheduler = await context.Scheduler.FindAsync(id);

            if (scheduler is null) return new NotFoundObjectResult("Agendamento não encontrado");

            if (scheduler.currStatus is not (Scheduler.Status.Done or Scheduler.Status.InProgress
                                                                   or Scheduler.Status.Canceled))
                return new BadRequestObjectResult("Não é possivel deletar um agendamento que ainda não foi realizado");

            context.Scheduler.Remove(scheduler);
            await context.SaveChangesAsync();
            return new OkObjectResult(scheduler);
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }
}