using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;

namespace APIx.Repositories;

public class UsersRepository(AppDBContext appDBContext)
{
    private readonly AppDBContext _appDBContext = appDBContext;

    public async Task<User?> RetrieveUserByCpf(string userCpf)
    {
        return await _appDBContext.User
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Cpf == userCpf);
    }
}