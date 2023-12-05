namespace AgilizAPI.Models;

public class Service
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid IdEstablishment { get; set; } = Guid.Empty!;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; } = default!;
    public string startTime { get; set; } = string.Empty;
    public string endTime { get; set; } = string.Empty;
    public int Duration { get; set; } = default!;
}

public static class ServicesExtensions
{
    public static ServicesDto ToDto(this Service service)
    {
        return new ServicesDto(service.Id, service.Name, service.Description, service.Price, service.Duration);
    }
}