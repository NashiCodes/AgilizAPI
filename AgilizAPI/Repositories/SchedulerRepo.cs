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
        var scheduler = await context.Scheduler.FindAsync(id).ConfigureAwait(false);

        return scheduler is null
                   ? new NotFoundObjectResult("Agendamento não encontrado")
                   : new OkObjectResult(scheduler);
    }

    public async Task<List<Scheduler>> GetSchedulerByService(Guid id)
    {
        return await context.Scheduler.Where(s => s.IdService == id).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<Scheduler>> GetSchedulerByUser(string id)
    {
        return await context.Scheduler.Where(s => s.IdUser == id).ToListAsync().ConfigureAwait(false);
    }

    public async Task<IActionResult> CreateScheduler(Scheduler scheduler)
    {
        var schedulersDb = context.Scheduler.Where(s => s.Date == scheduler.Date && s.IdService == scheduler.IdService);

        if (schedulersDb.Select(s => s.Hour == scheduler.Hour).Any())
            return new BadRequestObjectResult("Horario já reservado");

        try
        {
            await context.Scheduler.AddAsync(scheduler).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
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
            var schedulerDb = await context.Scheduler.FindAsync(id).ConfigureAwait(false);
            if (schedulerDb is null) return new NotFoundObjectResult("Agendamento não encontrado");
            if (await confereColisao(scheduler).ConfigureAwait(false))
                return new BadRequestObjectResult("Horario já reservado");

            schedulerDb.Date       = scheduler.Date;
            schedulerDb.IdService  = scheduler.IdService;
            schedulerDb.IdUser     = scheduler.IdUser;
            schedulerDb.currStatus = scheduler.currStatus;

            context.Scheduler.Update(schedulerDb);
            await context.SaveChangesAsync().ConfigureAwait(false);
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
            var scheduler = await context.Scheduler.FindAsync(id).ConfigureAwait(false);

            if (scheduler is null) return new NotFoundObjectResult("Agendamento não encontrado");

            if (scheduler.currStatus is not (Scheduler.Status.Done or Scheduler.Status.InProgress
                                                                   or Scheduler.Status.Canceled))
                return new BadRequestObjectResult("Não é possivel deletar um agendamento que ainda não foi realizado");

            context.Scheduler.Remove(scheduler);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return new OkObjectResult(scheduler);
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    private async Task<bool> confereColisao(Scheduler scheduler)
    {
        var schedulersDb = await context.Scheduler.Where(s => s.Date == scheduler.Date && s.IdService == scheduler
                                                                  .IdService)
                               .ToListAsync().ConfigureAwait(false);

        var count = schedulersDb.Select(s => s.Hour == scheduler.Hour).Count();

        return count >= 2;
    }
}