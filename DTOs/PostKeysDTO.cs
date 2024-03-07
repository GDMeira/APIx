using System.ComponentModel.DataAnnotations;

namespace APIx.DTOs;

public enum KeyType
{
    CPF,
    Email,
    Phone,
    Random
}

public class KeyDTO
{
    [Required]
    [EnumDataType(typeof(KeyType), ErrorMessage = "Invalid key type")]
    public required string Type { get; set; }
    public string? Value { get; set; }
}

public class UserDTO
{
    [Required]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF must have 11 characters")]
    public required string Cpf { get; set; }
}

public class AccountDTO
{
    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "Account number must be a number")]
    public required string Number { get; set; }

    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "Agency number must be a number")]
    public required string Agency { get; set; }
}
public class PostKeysDTO
{
    public required KeyDTO Key { get; set; }
    public required UserDTO User { get; set; }
    public required AccountDTO Account { get; set; }
}