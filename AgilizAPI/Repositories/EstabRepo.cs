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
        string thisCity = local[..3];

        List<Establishment> select = await context.Establishment.ToListAsync().ConfigureAwait(false);
        List<EstabDtoRaw> estabs = [];

        //Cria regex para verificar se o cep do estabelecimento é da mesma cidade do usuário

        foreach (Establishment? estab in select)
        {
            string estabCep = estab.Address[..3];
            if (CityComp(estabCep, thisCity)) estabs.Add(estab.ToDtoRaw());
        }

        return estabs.Count != 0
                   ? new OkObjectResult(estabs)
                   : new NotFoundObjectResult("Estabelecimento não encontrado");
    }

    private static bool CityComp(string? eLocal, string thisCity)
    {
        return eLocal == thisCity;
    }

    public async Task<IActionResult> GetEstab(Guid id, string Role)
    {
        Establishment? estab = await context.Establishment.FindAsync(id).ConfigureAwait(false);

        return estab is null
                   ? new NotFoundObjectResult("Estabelecimento não encontrado")
                   : new OkObjectResult(estab.ToDto(Role));
    }

    public async Task<IActionResult> LoginEstab(Guid id, string token)
    {
        Establishment? estb = await context.Establishment.FindAsync(id).ConfigureAwait(false);
        if (estb is null) return new NotFoundObjectResult("Estabelecimento não encontrado");

        return new OkObjectResult(estb.ToDto(token));
    }

    public async Task<IActionResult> GetEstabServices(Guid id)
    {
        List<ServicesDto> estabServices = await context.Services.Where(s => s.IdEstablishment.Equals(id)).Select(s => s.ToDto())
                                .ToListAsync().ConfigureAwait(false);


        return estabServices.Count != 0
                   ? new OkObjectResult(estabServices)
                   : new NotFoundObjectResult("Estabelecimento não encontrado");
    }


    public async Task<IActionResult> CadastrarEstab(EstabRegister estab)
    {
        User? userbDb = await context.User.FindAsync(estab.Email).ConfigureAwait(false);

        if (userbDb is null) return new NotFoundObjectResult("Email não cadastrado");

        bool existeEstab =
            await context.Establishment.Where(e => e.Email == estab.Email).AnyAsync().ConfigureAwait(false);

        if (existeEstab) return new BadRequestObjectResult("Estabelecimento já cadastrado");

        Establishment newEstab = new Establishment().register(estab, userbDb.Password);

        await context.Establishment.AddAsync(newEstab).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);

        return new CreatedAtActionResult("GetEstablishment", "Establishments", new { id = newEstab }, estab);
    }


    public async Task<IActionResult> EditarEstab(Guid id, Establishment estab)
    {
        if (id != estab.Id) return new BadRequestResult();

        Establishment? estabDb = await context.Establishment.FindAsync(id).ConfigureAwait(false);
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

    public static async Task<JObject> ViaCepJson(string cep)
    {
        string url = $"https://viacep.com.br/ws/{cep}/json/";

        HttpClient client = new();
        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
        };

        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        JObject json = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

        return json;
    }

    public async Task<IActionResult> DeletarEstab(Guid id, string email)
    {
        Establishment? estab = await context.Establishment.FindAsync(id).ConfigureAwait(false);

        if (estab is null) return new NotFoundObjectResult("Estabelecimento não encontrado");

        if (estab.Email != email) return new BadRequestObjectResult("Email incorreto");

        context.Establishment.Remove(estab);
        await context.SaveChangesAsync().ConfigureAwait(false);

        return new OkObjectResult("Estabelecimento deletado com sucesso");
    }
}