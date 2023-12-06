#region

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NuGet.ProjectModel;

#endregion

namespace AgilizAPI.Repositories;

public interface IUsersRepo
{
    public Task<IActionResult> CadastrarUser(UserRegister requestUser);
    public Task<IActionResult> Login(UserLogin            user);

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

        using var response = await client.SendAsync(request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var json  = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        var valid = json.GetValue<string>("valid");

        if ("False".Equals(valid))
        {
            var reason = json.GetValue<string>("reason");
            var msg    = $"Email inválido \n Razão : \"{reason}\"";
            throw new FormatException(msg);
        }
    }

    protected static async Task ValidateProperties(UserRegister user)
    {
        var properties = user.GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(user);
            if (value is not null) continue;
            var msg = $"Propriedade {property.Name} não pode ser nula";
            throw new FormatException(msg);
        }

        try
        {
            await ValidateEmail(user.Email);
            validatePhone(user.UserPhone);
            validateCpf(user.UserCpf);
            validateCep(user.UserAddress);
            validateUserName(user.UserName);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void validatePhone(string phone)
    {
        //remove os caracteres especiais
        phone = Regex.Replace(phone, @"[^\d]", "");

        if (phone.Length != 11) throw new FormatException("Telefone inválido");

        //Respeita o formato (xx) xxxxx-xxxx
        phone = $"({phone[..2]}) {phone.Substring(2, 5)}-{phone.Substring(7, 4)}";

        var regex = new Regex(@"^\([1-9]{2}\) (?:[2-8]|9[0-9])[0-9]{3}\-[0-9]{4}$");
        if (regex.IsMatch(phone)) return;
        const string msg = "Telefone inválido";
        throw new FormatException(msg);
    }

    private static void validateCpf(string cpf)
    {
        //remove os caracteres especiais
        cpf = Regex.Replace(cpf, @"[^\d]", "");

        if (cpf.Length != 11) throw new FormatException("CPF inválido");

        //Respeita o formato xxx.xxx.xxx-xx
        cpf = $"{cpf[..3]}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";

        var regex = new Regex(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$");
        if (regex.IsMatch(cpf)) return;
        const string msg = "CPF inválido";
        throw new FormatException(msg);
    }

    private static void validateCep(string cep)
    {
        //remove os caracteres especiais
        cep = Regex.Replace(cep, @"[^\d]", "");

        if (cep.Length != 8) throw new FormatException("CEP inválido");

        //Respeita o formato xxxxx-xxx
        cep = $"{cep[..5]}-{cep.Substring(5, 3)}";

        var regex = new Regex(@"^\d{5}\-\d{3}$");
        if (regex.IsMatch(cep)) return;
        const string msg = "CEP inválido";
        throw new FormatException(msg);
    }

    private static void validateUserName(string name)
    {
        if (name.Length < 3) throw new FormatException("Nome inválido");

        var regex = new Regex(@"^([ \u00c0-\u01ffa-zA-Z'\-])+$");
        if (regex.IsMatch(name)) return;

        const string msg = "Nome inválido";
        throw new FormatException(msg);
    }
}