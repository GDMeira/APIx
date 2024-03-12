using APIx.Models;

namespace APIx.ResponseDTOs;

public class FullUserDTO(string name, string cpf)
{
    public string Name { get; set; } = name;
    public string MaskedCpf { get; set; } = MaskCpf(cpf);

    public static string MaskCpf(string cpf)
    {
        return string.Concat(cpf.AsSpan(0, 3), "***.***.***-", cpf.AsSpan(9, 2));
    }
}