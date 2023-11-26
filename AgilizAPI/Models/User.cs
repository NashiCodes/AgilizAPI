﻿using AgilizaAppAPI;
using System.ComponentModel.DataAnnotations;

namespace AgilizAPI.Models;

public class User
{
    [Key]
    [Required]
    [MaxLength(50)]
    [MinLength(7)]
    public required string email { get; set; }

    [Required][MinLength(8)] public required string password { get; set; }

    [Required][MaxLength(50)] public required string name { get; set; }

    [Required] public required string phone { get; set; }

    [Required]
    [MaxLength(11)]
    [MinLength(11)]
    public required string cpf { get; set; }

    [StringLength(8)][MinLength(8)] public string address { get; set; } = "";

    [StringLength(4)] public string addressNumber { get; set; } = "";
}

public static class UserExtensions
{
    public static userToDto ToDto(this User user)
    {
        return new userToDto(user.name, user.phone, user.address, user.addressNumber);
    }
}