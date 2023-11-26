using System.ComponentModel.DataAnnotations;

namespace AgilizaAppAPI.Models;

public class Establishment
{
    [Key][Required] public required int Id { get; set; }

    [Required][MaxLength(100)] public required string Name { get; set; }

    [Required][MaxLength(100)] public required string Category { get; set; }

    [Required][MaxLength(100)] public required string Address { get; set; }

    [Required][MaxLength(100)] public required string City { get; set; }

    [Required][MaxLength(100)] public required string State { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8)]
    public required string Password { get; set; }
}