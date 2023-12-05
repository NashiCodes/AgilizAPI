#region

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using static System.DateTime;

#endregion

namespace AgilizAPI.Models;

public class User
{
    public enum Roles
    {
        User,
        Entrepreneur
    }

    public string Email { get; set; } = string.Empty;
    public byte[] Password { get; set; } = default!;
    public byte[] PasswordSalt { get; set; } = default!;
    public string UserName { get; set; } = string.Empty;
    public string UserPhone { get; set; } = string.Empty;
    public string UserCpf { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.User.ToString();
    public string UserAddress { get; set; } = string.Empty;
    public string addressNumber { get; set; } = string.Empty;

    public void CreatePasswordHash(string password)
    {
        using HMACSHA512 hmac = new();
        this.PasswordSalt = hmac.Key;
        this.Password     = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public bool VerifyPassword(string password)
    {
        using HMACSHA512 hmac         = new(this.PasswordSalt);
        var              computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        var              result       = computedHash.SequenceEqual(this.Password);
        return result;
    }

    public string GenerateToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key          = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("PRIVATE_KEY")!);
        var claim = new List<Claim> {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, this.Email),
            new(JwtRegisteredClaimNames.Sub, this.Email),
            new("Role", this.Role)
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

public static class UserExtensions
{
    public static User toUser(this UserRegister userRegister)
    {
        var user = new User {
            Email         = userRegister.Email,
            UserName      = userRegister.UserName,
            UserPhone     = userRegister.UserPhone,
            UserCpf       = userRegister.UserCpf,
            UserAddress   = userRegister.UserAddress,
            addressNumber = userRegister.AddressNumber,
            Role          = userRegister.Role
        };
        user.CreatePasswordHash(userRegister.Password);
        return user;
    }

    public static UserToDto ToDto(this User user, string token, List<Scheduler> scheduler)
    {
        return new UserToDto(user.UserName, user.UserPhone, user.UserAddress, user.addressNumber, token, scheduler);
    }
}