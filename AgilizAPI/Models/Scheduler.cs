namespace AgilizAPI.Models;

public class Scheduler
{
    public enum Status
    {
        Waiting,
        InProgress,
        Done,
        Canceled
    }

    public Guid Id { get; set; }

    public Status currStatus { get; set; } = Status.Waiting;
    public string Date { get; set; } = string.Empty;
    public string Hour { get; set; } = string.Empty;
    public Guid IdService { get; set; } = Guid.Empty!;
    public string IdUser { get; set; } = string.Empty;
}