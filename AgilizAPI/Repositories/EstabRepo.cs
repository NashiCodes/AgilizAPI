#region

using AgilizAPI.Data;
using AgilizAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AgilizAPI.Repositories;

public class EstabRepo(AgilizApiContext context)
{
    public async Task<ActionResult<IEnumerable<EstablishmentDto>>> GetAll()
    {
        return await context.Establishment.Select(e => e.ToDto("User")).ToListAsync();
    }

    public async Task<IActionResult> GetEstab(Guid id, string Role)
    {
        return new OkObjectResult(await context.Establishment.FindAsync(id));
    }


    public async Task<IActionResult> GetEstabServices(Guid id)
    {
        var estabServices = await context.Services.Where(s => s.IdEstablishment.Equals(id)).Select(s => s.ToDto())
                                .ToListAsync();

        return estabServices.Any()
                   ? new OkObjectResult(estabServices)
                   : new NotFoundObjectResult("Estabelecimento não encontrado");
    }


    public async Task<IActionResult> CadastrarEstab(Establishment estab)
    {
        context.Establishment.Add(estab);
        await context.SaveChangesAsync();

        return new CreatedAtActionResult("GetEstablishment", "Establishments", new { id = estab.Id }, estab);
    }


    public async Task<IActionResult> EditarEstab(Guid id, Establishment estab)
    {
        if (id != estab.Id) return new BadRequestResult();

        var estabDb = await context.Establishment.FindAsync(id);
        if (estabDb is null) return new NotFoundResult();

        if (!estabDb.VerifyPassword(estab.Password))
            return new BadRequestResult();

        context.Entry(estab).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EstablishmentExists(id))
                return new NotFoundResult();
            return new BadRequestResult();
        }

        return new OkObjectResult("Estabelecimento editado com sucesso");
    }


    private bool EstablishmentExists(Guid id)
    {
        return context.Establishment.Any(e => e.Id == id);
    }
}