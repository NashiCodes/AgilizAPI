#region

using AgilizAPI.Models;

#endregion

namespace AgilizAPI;

public record UserLogin(string Email, string Password); // Objeto pra ser enviado ao login

public record UserRegister(string Email,       string Password,      string UserName, string UserPhone, string UserCpf,
    string                        UserAddress, string AddressNumber, string Role);

public record UserToDto(string Name, string Phone, string UserAddress, string AddressNumber, string Token,
    List<Scheduler>            Scheduler);

public record EstabDtoRaw(Guid id, string name, string category, LocationDto location);

public record EstabRegister(string Email, string Name, string Category, string Address, string AddressNumber);

public record ServicesDto(Guid Id, string Name, string Description, double Price, int Duration);

public record EstablishmentDto(EstabDtoRaw Estab, List<ServicesDto> Services, string Token);

public record LocationDto(string Cep, string City, string Street, string Neighborhood, string Number);