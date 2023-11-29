#region

using System.ComponentModel.DataAnnotations;

#endregion

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

    [Key][Required] public required int Id { get; set; }

    public Status currStatus { get; set; } = Status.Waiting;
    [Required] public required string Date { get; set; }
    [Required] public required string Hour { get; set; }
    [Required] public required int IdService { get; set; }
    [Required] public required int IdUser { get; set; }
}