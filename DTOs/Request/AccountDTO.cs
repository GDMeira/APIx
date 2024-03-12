using System.ComponentModel.DataAnnotations;

namespace APIx.RequestDTOs;
public class AccountDTO
{
    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "Account number must be a number")]
    public required string Number { get; set; }

    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "Agency number must be a number")]
    public required string Agency { get; set; }
}