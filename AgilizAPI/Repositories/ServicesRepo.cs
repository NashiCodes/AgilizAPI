#region

using AgilizAPI.Data;
using AgilizAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AgilizAPI.Repositories;

public class ServicesRepo(AgilizApiContext context)
{
    public async Task<ActionResult> GetServiceOnly(Guid id)
    {
        var service = await context.Services.FindAsync(id).ConfigureAwait(false);

        return service is null
                   ? new NotFoundResult()
                   : new OkObjectResult(service.ToDto());
    }

    public async Task<IActionResult> GetServicesEstab(Guid estabId)
    {
        var estab = await context.Establishment.FindAsync(estabId).ConfigureAwait(false);
        if (estab is null) return new NotFoundObjectResult("Estabelecimento não encontrado");

        var services = await context.Services.Where(s => s.IdEstablishment.Equals(estabId)).ToListAsync()
                           .ConfigureAwait(false);

        return new OkObjectResult(services.Select(s => s.ToDto()));
    }

    public async Task<IActionResult> CreateService(Service service)
    {
        try
        {
            await context.Services.AddAsync(service).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return new OkObjectResult(service.ToDto());
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    public async Task<IActionResult> EditService(Guid id, Service service)
    {
        try
        {
            var serviceDb = await context.Services.FindAsync(id).ConfigureAwait(false);
            if (serviceDb is null) return new NotFoundObjectResult("Serviço não encontrado");

            serviceDb.Name        = service.Name;
            serviceDb.Description = service.Description;
            serviceDb.Price       = service.Price;
            serviceDb.Duration    = service.Duration;

            context.Services.Update(serviceDb);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return new OkObjectResult(serviceDb.ToDto());
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    public async Task<IActionResult> DeleteService(Guid id)
    {
        var service = await context.Services.FindAsync(id).ConfigureAwait(false);
        if (service == null) return new NotFoundObjectResult("Serviço não encontrado");

        var schedulers = await new SchedulerRepo(context).GetSchedulerByService(id).ConfigureAwait(false);
        if (schedulers.Count > 0)
            foreach (var scheduler in schedulers)
                context.Scheduler.Remove(scheduler);


        context.Services.Remove(service);
        await context.SaveChangesAsync().ConfigureAwait(false);

        return new OkObjectResult("Serviço deletado com sucesso");
    }
}