#region

using AgilizAPI.Models;

#endregion

namespace AgilizAPI;

public record UserLogin(string Email, string Password);

public record UserRegister(string Email,       string Password,      string UserName, string UserPhone, string UserCpf,
    string                        UserAddress, string AddressNumber, string Role);

public record UserToDto(string Name, string Phone, string UserAddress, string AddressNumber, string Token,
    List<Scheduler>            Scheduler);

public record EstabDtoRaw(Guid id, string name, string category, string address, string addressNumber);

public record ServicesDto(Guid Id, string Name, string Description, double Price, int Duration);

public record EstablishmentDto(EstabDtoRaw Estab, List<ServicesDto> Services, string Token);