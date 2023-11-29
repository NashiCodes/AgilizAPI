#region

using System.ComponentModel.DataAnnotations;

#endregion

namespace AgilizAPI.Models;

public class Services
{
    [Key][Required] public required int Id { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required int IdEstablishment { get; set; }
    [Required] public required string Description { get; set; }
    [Required] public required double Price { get; set; }
    [Required] public required string startTime { get; set; }
    [Required] public required string endTime { get; set; }
    [Required] public required int Duration { get; set; }
}