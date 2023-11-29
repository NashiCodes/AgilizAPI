#region

using AgilizAPI.Data;
using AgilizAPI.Models;
using Newtonsoft.Json.Linq;
using NuGet.ProjectModel;

#endregion

namespace AgilizAPI.Repositories;

public class UsersRepo(AgilizApiContext context) : IUsersRepo
{
    public async Task<IResult> CadastrarUser(User user)
    {
        try
        {
            await Task.Run(() => ValidateEmail(user.email));
            await context.User.AddAsync(user);
            await context.SaveChangesAsync();
            return Results.Ok("Usuario Cadastrado com sucesso");
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    public async Task<IResult> Login(UserDto user)
    {
        try
        {
            await ValidateEmail(user.email);

            var userDb = context.User.Find(user.email);

            if (userDb != null)
            {
                if (userDb.password == user.password)
                {
                    if (userDb.isEnterpreneur)
                        return LoadEstablishment(userDb);

                    return Results.Ok(userDb.ToDto());
                }

                return Results.BadRequest("Senha incorreta");
            }

            return Results.BadRequest("Usuario não cadastrado");
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    private async Task ValidateEmail(string email)
    {
        var client  = new HttpClient();
        var request = new HttpRequestMessage();
        request.Method     = HttpMethod.Get;
        request.RequestUri = new Uri($"https://mailcheck.p.rapidapi.com/?domain={email}");
        request.Headers.Add("X-RapidAPI-Host", "mailcheck.p.rapidapi.com");
        //Get rapidkey from user secrets
        var RapidKey = Environment.GetEnvironmentVariable("RapidKey");
        request.Headers.Add("X-RapidAPI-Key", RapidKey);

        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json  = JObject.Parse(await response.Content.ReadAsStringAsync());
        var valid = json.GetValue<string>("valid");

        if ("False".Equals(valid))
        {
            var reason    = json.GetValue<string>("reason");
            var msg       = $"Email inválido \n Razão : \"{reason}\"";
            var exception = new Exception(msg);
            throw exception;
        }
    }

    private IResult LoadEstablishment(User dbUser)
    {
        var estabishment = context.Establishment.Where(e => e.email == dbUser.email || e.Password == dbUser.password);

        return estabishment.Any()
                   ? Results.Ok(estabishment.First())
                   : Results.BadRequest("Estabelecimento Não encontrado \n, contate o suporte para mais informações");
    }
}