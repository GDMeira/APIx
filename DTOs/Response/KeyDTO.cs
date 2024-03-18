using APIx.Models;

namespace APIx.ResponseDTOs;

public class KeyDTO(string type, string value)
{
    public string Type { get; set; } = type;
    public string Value { get; set; } = value;
}