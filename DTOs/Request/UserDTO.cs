using System.ComponentModel.DataAnnotations;

namespace APIx.RequestDTOs;

public class UserDTO
{
    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF must have 11 number digits")]
    public required string Cpf { get; set; }
}