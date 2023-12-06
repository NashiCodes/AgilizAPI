#region

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AgilizAPI.Repositories;
using Newtonsoft.Json.Linq;
using NuGet.ProjectModel;

#endregion

namespace AgilizAPI.Models;

public class Establishment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; }

    [Required] public string Email { get; set; } = default!;
    [Required] public string Name { get; set; } = default!;
    [Required] public string Category { get; set; } = default!;
    [Required] public string Address { get; set; } = default!;
    [Required] public string AddressNumber { get; set; } = default!;
    [Required] public byte[] Password { get; set; } = default!;

    public JObject addressJson { get; set; } = default!;

    public Establishment register(EstabRegister estab, byte[] pass)
    {
        this.Email         = estab.Email;
        this.Name          = estab.Name;
        this.Category      = estab.Category;
        this.Address       = estab.Address;
        this.AddressNumber = estab.AddressNumber;
        this.Password      = pass;

        return this;
    }

    public bool VerifyPassword(IEnumerable<byte> password)
    {
        return password.SequenceEqual(this.Password);
    }

    public string getCity()
    {
        return this.addressJson.GetValue<string>("localidade");
    }

    public string getRua()
    {
        return this.addressJson.GetValue<string>("logradouro");
    }

    public string getBairro()
    {
        return this.addressJson.GetValue<string>("bairro");
    }

    public void loadJson()
    {
        this.addressJson = EstabRepo.viaCepJson(this.Address).Result;
    }
}

public static class EstablishmentExtensions
{
    public static EstablishmentDto ToDto(this Establishment establishment, string token)
    {
        return new EstablishmentDto(establishment.ToDtoRaw(), new List<ServicesDto>(), token);
    }

    public static EstabDtoRaw ToDtoRaw(this Establishment establishment)
    {
        establishment.loadJson();

        var location = new LocationDto(establishment.Address, establishment.getCity(), establishment.getRua(),
                                       establishment.getBairro(), establishment.AddressNumber);

        return new EstabDtoRaw(establishment.Id, establishment.Name, establishment.Category, location);
    }
}