using System.ComponentModel.DataAnnotations;

namespace APIx.RequestDTOs;
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
    public required string Value { get; set; }
}