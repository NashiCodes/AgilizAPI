#region

using AgilizAPI.Data;
using AgilizAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

#endregion

namespace AgilizAPI.Repositories;

public class EstabRepo(AgilizApiContext context)
{
    public async Task<ActionResult<IEnumerable<EstabDtoRaw>>> GetAll(string local)
    {
        var json = await viaCepJson(local).ConfigureAwait(false);

        var thisCity = json["localidade"]?.ToString();

        var select = await context.Establishment.Select(e => e.ToDtoRaw()).ToListAsync().ConfigureAwait(false);

        var estabs = select.Where(e => thisCity != null && cityComp(e, thisCity)).ToList();

        return estabs.Any()
                   ? new OkObjectResult(estabs)
                   : new NotFoundObjectResult("Estabelecimento não encontrado");
    }

    private static bool cityComp(EstabDtoRaw e, string thisCity)
    {
        return e.location.City == thisCity;
    }

    public async Task<IActionResult> GetEstab(Guid id, string Role)
    {
        var estab = await context.Establishment.FindAsync(id).ConfigureAwait(false);

        return estab is null
                   ? new NotFoundObjectResult("Estabelecimento não encontrado")
                   : new OkObjectResult(estab.ToDto(Role));
    }

    public async Task<IActionResult> loginEstab(Guid id, string token)
    {
        var estb = await context.Establishment.FindAsync(id).ConfigureAwait(false);
        if (estb is null) return new NotFoundObjectResult("Estabelecimento não encontrado");

        return new OkObjectResult(estb.ToDto(token));
    }

    public async Task<IActionResult> GetEstabServices(Guid id)
    {
        var estabServices = await context.Services.Where(s => s.IdEstablishment.Equals(id)).Select(s => s.ToDto())
                                .ToListAsync().ConfigureAwait(false);

        return estabServices.Any()
                   ? new OkObjectResult(estabServices)
                   : new NotFoundObjectResult("Estabelecimento não encontrado");
    }


    public async Task<IActionResult> CadastrarEstab(EstabRegister estab)
    {
        var userbDb = await context.User.FindAsync(estab.Email).ConfigureAwait(false);

        if (userbDb is null) return new NotFoundObjectResult("Email não cadastrado");

        var newEstab = new Establishment().register(estab, userbDb.Password);

        await context.Establishment.AddAsync(newEstab).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);

        return new CreatedAtActionResult("GetEstablishment", "Establishments", new { id = newEstab }, estab);
    }


    public async Task<IActionResult> EditarEstab(Guid id, Establishment estab)
    {
        if (id != estab.Id) return new BadRequestResult();

        var estabDb = await context.Establishment.FindAsync(id).ConfigureAwait(false);
        if (estabDb is null) return new NotFoundResult();

        if (!estabDb.VerifyPassword(estab.Password))
            return new BadRequestObjectResult("Senha incorreta");

        if (estab.Email != estabDb.Email)
            return new BadRequestObjectResult("Não é possível alterar o email do estabelecimento");

        context.Entry(estab).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync().ConfigureAwait(false);
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

    public static async Task<JObject> viaCepJson(string cep)
    {
        var url = $"https://viacep.com.br/ws/{cep}/json/";

        HttpClient client = new();
        HttpRequestMessage request = new() {
            Method     = HttpMethod.Get,
            RequestUri = new Uri(url)
        };

        var response = await client.SendAsync(request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var json = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

        return json;
    }
}