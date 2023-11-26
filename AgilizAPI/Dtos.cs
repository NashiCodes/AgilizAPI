namespace AgilizaAppAPI;

public record userDto(string email, string password);

public record userToDto(string name, string phone, string address, string addressNumber);