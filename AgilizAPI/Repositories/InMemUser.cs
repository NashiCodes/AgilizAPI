using AgilizaAppAPI;
using AgilizAPI.Models;

namespace AgilizAPI.Repositories;

public class InMemUser : IUsersRepo
{
    private static readonly Dictionary<string, User> users = new()
    {
        {
            "pereiraanjos11@gmail.com", new User
            {
                email = "pereiraanjos11@gmail.com",
                password = "12345",
                name = "Joao",
                phone = "33999605766",
                cpf = "12555353690"
            }
        }
    };

    public async Task<IResult> CadastrarUser(User user)
    {
        try
        {
            await Task.Run(() => validateEmail(user.email));
            await Task.Run(() => users.Add(user.email, user));
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

            if (users.ContainsKey(user.email))
            {
                if (users[user.email].password == user.password)
                    return Results.Ok(users[user.email].ToDto());
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
        var body = await response.Content.ReadAsStringAsync();
        if (body.Contains("E-mail address is blacklisted")) throw new Exception("E-mail address is blacklisted");
    }
}