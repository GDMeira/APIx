using APIx.Models;

namespace APIx.ResponseDTOs;

public class KeyDTO(PixKey pixKey)
{
    public string Type { get; set; } = pixKey.Type;
    public string Value { get; set; } = pixKey.Value;
}