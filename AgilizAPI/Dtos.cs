namespace AgilizAPI;

public record UserLogin(string Email, string Password);

public record UserRegister(string Email,       string Password, string UserName, string UserPhone, string UserCpf,
    string                        UserAddress, string AddressNumber);

public record UserToDto(string Name, string Phone, string UserAddress, string AddressNumber, string Token);

public record EstablishmentDto(Guid id, string name, string category, string address, string addressNumber);