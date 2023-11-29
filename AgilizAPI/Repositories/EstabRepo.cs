#region

using AgilizAPI.Data;
using AgilizAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AgilizAPI.Repositories;

public class EstabRepo(AgilizApiContext context) : IEstabRepo
{
    public async Task<ActionResult<IEnumerable<EstablishmentDto>>> GetAll()
    {
        return await context.Establishment.Select(e => e.ToDto()).ToListAsync();
    }


    public Task<ActionResult<EstablishmentDto>> GetEstabServices(Guid id)
    {
        throw new NotImplementedException();
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

        context.Entry(estab).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EstablishmentExists(id))
                return new NotFoundResult();
            throw;
        }

        return new NoContentResult();
    }


    private bool EstablishmentExists(Guid id)
    {
        return context.Establishment.Any(e => e.Id == id);
    }
}