using AgilizaAppAPI;
using AgilizAPI.Data;
using AgilizAPI.Models;
using Newtonsoft.Json.Linq;

namespace AgilizAPI.Repositories;

public class UsersRepo(APIContext context) : IUsersRepo
{
    public async Task<IResult> CadastrarUser(User user)
    {
        try
        {
            await Task.Run(() => validateEmail(user.email));
            await context.User.AddAsync(user);
            await context.SaveChangesAsync();
            return Results.Ok("Usuario Cadastrado com sucesso");
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    public IResult login(userDto user)
    {
        try
        {
            validateEmail(user.email);

            var userDb = context.User.Find(user.email);

            if (userDb != null)
            {
                if (userDb.password == user.password)
                    return Results.Ok(userDb.ToDto());
                return Results.BadRequest("Senha incorreta");
            }

            return Results.BadRequest("Usuario não cadastrado");
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    private async void validateEmail(string email)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://mailcheck.p.rapidapi.com/?domain=" + email),
            Headers =
            {
                { "X-RapidAPI-Key", "573e32a749msh766f31fba16007ap189ad7jsn7e2c2ca78842" },
                { "X-RapidAPI-Host", "mailcheck.p.rapidapi.com" }
            }
        };

        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = JObject.Parse(await response.Content.ReadAsStringAsync());
        var valid = ((JObject)json["valid"]!).ToString();
        if (valid == "false")
        {
            var reason = ((JObject)json["reason"]!).ToString();
            var msg = "Email inválido por razão de: \"".Concat(reason).Concat("\"");
            throw new Exception(msg.ToString());
        }
    }
}