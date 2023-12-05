#region

using AgilizAPI.Data;
using AgilizAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AgilizAPI.Repositories;

public class EstabRepo(AgilizApiContext context)
{
    public async Task<ActionResult<IEnumerable<EstablishmentDto>>> GetAll(string local)
    {
        var estabs = await context.Establishment.ToListAsync();

        var thisCity = await Establishment.getCity(local);

        var cepDict = await CepDecoder(estabs);

        return cepDict.TryGetValue(thisCity, out var estabList)
                   ? new OkObjectResult(estabList)
                   : new NotFoundObjectResult("Nenhum estabelecimento encontrado");
    }

    public async Task<IActionResult> GetEstab(Guid id, string Role)
    {
        var estab = await context.Establishment.FindAsync(id);

        return estab is null
                   ? new NotFoundObjectResult("Estabelecimento não encontrado")
                   : new OkObjectResult(estab.ToDto(Role));
    }

    public async Task<IActionResult> loginEstab(Guid id, string token)
    {
        var estb = await context.Establishment.FindAsync(id);
        if (estb is null) return new NotFoundObjectResult("Estabelecimento não encontrado");

        return new OkObjectResult(estb.ToDto(token));
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

    private static async Task<Dictionary<string, List<EstabDtoRaw>>> CepDecoder(List<Establishment> estabs)
    {
        var cepDict = new Dictionary<string, List<EstabDtoRaw>>();
        foreach (var estab in estabs)
        {
            var city = await estab.getCity();
            if (cepDict.ContainsKey(city))
                cepDict[city].Add(estab.ToDtoRaw(city));
            else
                cepDict.Add(city, new List<EstabDtoRaw> { estab.ToDtoRaw(city) });
        }

        return cepDict;
    }
}