#region

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace AgilizAPI.Models;

public class Establishment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(7)]
    public required string email { get; set; }

    [Required] [MaxLength(100)] public required string Name { get; set; }

    [Required] [MaxLength(100)] public required string Category { get; set; }

    [Required]
    [StringLength(8)]
    [MinLength(8)]
    public required string Address { get; set; }

    [StringLength(4)] public string addressNumber { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8)]
    public required string Password { get; set; }
}

public static class EstablishmentExtensions
{
    public static EstablishmentDto ToDto(this Establishment establishment)
    {
        return new EstablishmentDto(establishment.Id, establishment.Name, establishment.Category, establishment.Address,
                                    establishment.addressNumber);
    }
}