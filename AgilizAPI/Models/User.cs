#region

using System.Security.Cryptography;
using System.Text;

#endregion

namespace AgilizAPI.Models;

public class User
{
    public string Email { get; set; } = string.Empty;
    public byte[] Password { get; set; } = default!;
    public byte[] PasswordSalt { get; set; } = default!;
    public string UserName { get; set; } = string.Empty;
    public string UserPhone { get; set; } = string.Empty;
    public string UserCpf { get; set; } = string.Empty;
    public UserClaims Role { get; set; } = UserClaims.CommonUser;
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
        return computedHash.SequenceEqual(this.Password);
    }
}

public enum UserClaims
{
    CommonUser,
    Entrepreneur
}

public static class UserExtensions
{
    public static User toUser(this UserRegister userRegister)
    {
        return new User {
            Email         = userRegister.Email,
            UserName      = userRegister.UserName,
            UserPhone     = userRegister.UserPhone,
            UserCpf       = userRegister.UserCpf,
            UserAddress   = userRegister.UserAddress,
            addressNumber = userRegister.AddressNumber
        };
    }

    public static UserToDto ToDto(this User user, string token)
    {
        return new UserToDto(user.UserName, user.UserPhone, user.UserAddress, user.addressNumber, token);
    }
}