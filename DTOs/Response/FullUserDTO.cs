using APIx.Models;

namespace APIx.ResponseDTOs;

public class FullUserDTO(User user)
{
    public string Name { get; set; } = user.Name;
    public string MaskedCpf { get; set; } = MaskCpf(user.Cpf);

    public static string MaskCpf(string cpf)
    {
        return string.Concat(cpf.AsSpan(0, 3), "***.***.***-", cpf.AsSpan(9, 2));
    }
}