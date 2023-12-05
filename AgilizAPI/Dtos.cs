#region

using AgilizAPI.Models;

#endregion

namespace AgilizAPI;

public record UserLogin(string Email, string Password);

public record UserRegister(string Email,       string Password,      string UserName, string UserPhone, string UserCpf,
    string                        UserAddress, string AddressNumber, string Role);

public record UserToDto(string Name, string Phone, string UserAddress, string AddressNumber, string Token,
    List<Scheduler>            Scheduler);

public record EstablishmentDto(Guid id, string name, string category, string address, string addressNumber,
    string                          token);

public record ServicesDto(Guid Id, string Name, string Description, double Price, int Duration);