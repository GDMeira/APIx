using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;

namespace APIx.Repositories;

public class ConcilliationRepository(AppDBContext appDBContext, CacheRepository cacheRepository)
{
    private readonly AppDBContext _appDBContext = appDBContext;
    private readonly CacheRepository _cacheRepository = cacheRepository;

    public async Task<Concilliation> CreateConcilliation(Concilliation concilliation)
    {
        await _appDBContext.Concilliation
            .AddAsync(concilliation);
        await _appDBContext.SaveChangesAsync();

        await _cacheRepository
            .SetCachedData($"concilliation-{concilliation.Id}", concilliation);

        return concilliation;
    }

    public async Task<Concilliation?> RetrieveConcilliationByFileUrl(string fileUrl)
    {
        Concilliation? concilliationDB = await _appDBContext.Concilliation
            .FirstOrDefaultAsync(c => c.FileUrl == fileUrl);

        return concilliationDB;
    }
}