#region

using Newtonsoft.Json.Linq;
using NuGet.ProjectModel;

#endregion

namespace AgilizAPI.Models;

public class Establishment
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string AddressNumber { get; set; } = string.Empty;
    public byte[] Password { get; set; } = default!;

    public bool VerifyPassword(IEnumerable<byte> password)
    {
        return password.SequenceEqual(this.Password);
    }

    private static JObject viaCepJson(string cep)
    {
        var url = $"https://viacep.com.br/ws/{cep}/json/";

        HttpClient client = new();
        HttpRequestMessage request = new() {
            Method     = HttpMethod.Get,
            RequestUri = new Uri(url)
        };

        using var response = client.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();

        return JObject.Parse(response.Content.ReadAsStringAsync().Result);
    }

    public static async Task<string> getCity(string cep)
    {
        var json = await Task.Run(() => viaCepJson(cep));
        return json.GetValue<string>("localidade");
    }

    public async Task<string> getCity()
    {
        return await getCity(this.Address);
    }

    public string getRua()
    {
        return viaCepJson(this.Address).GetValue<string>("logradouro");
    }
}

public static class EstablishmentExtensions
{
    public static EstablishmentDto ToDto(this Establishment establishment, string token)
    {
        return new EstablishmentDto(establishment.ToDtoRaw(establishment.Address), new List<ServicesDto>(), token);
    }

    public static EstabDtoRaw ToDtoRaw(this Establishment establishment, string Address)
    {
        return new EstabDtoRaw(establishment.Id, establishment.Name, establishment.Category, Address,
                               establishment.AddressNumber);
    }
}