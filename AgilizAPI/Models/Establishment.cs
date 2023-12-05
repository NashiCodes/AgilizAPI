#region

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using static System.DateTime;

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

    public bool VerifyPassword(byte[] password)
    {
        return password.SequenceEqual(this.Password);
    }

    public string GenerateToken(string Role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key          = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("PRIVATE_KEY")!);
        var claim = new List<Claim> {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, this.Email),
            new(JwtRegisteredClaimNames.Sub, this.Email),
            new("Role", Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claim),
            Expires = UtcNow.AddHours(2),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public static class EstablishmentExtensions
{
    public static EstablishmentDto ToDto(this Establishment establishment, string token)
    {
        return new EstablishmentDto(establishment.Id, establishment.Name, establishment.Category, establishment.Address,
                                    establishment.AddressNumber, token);
    }
}