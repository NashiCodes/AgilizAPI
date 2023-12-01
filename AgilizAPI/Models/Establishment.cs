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

    public bool VerifyPassword(byte[] password)
    {
        return password.SequenceEqual(this.Password);
    }
}

public static class EstablishmentExtensions
{
    public static EstablishmentDto ToDto(this Establishment establishment)
    {
        return new EstablishmentDto(establishment.Id, establishment.Name, establishment.Category, establishment.Address,
                                    establishment.AddressNumber);
    }
}