namespace AgilizAPI;

public record UserDto(string email, string password);

public record UserToDto(string name, string phone, string address, string addressNumber);

public record EstablishmentDto(Guid id, string name, string category, string address, string addressNumber);