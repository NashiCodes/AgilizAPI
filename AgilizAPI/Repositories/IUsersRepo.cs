#region

using Newtonsoft.Json.Linq;
using NuGet.ProjectModel;

#endregion

namespace AgilizAPI.Repositories;

public interface IUsersRepo
{
    public Task<IResult> CadastrarUser(UserRegister requestUser);
    public Task<IResult> Login(UserLogin            user);

    protected static async Task ValidateEmail(string email)
    {
        HttpClient client = new();
        HttpRequestMessage request = new() {
            Method     = HttpMethod.Get,
            RequestUri = new Uri($"https://mailcheck.p.rapidapi.com/?domain={email}")
        };
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
            var       reason    = json.GetValue<string>("reason");
            var       msg       = $"Email inválido \n Razão : \"{reason}\"";
            Exception exception = new(msg);
            throw exception;
        }
    }
}