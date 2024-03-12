using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;

namespace APIx.Repositories;

public class PaymentsRepository(AppDBContext appDBContext)
{
    private readonly AppDBContext _appDBContext = appDBContext;
    public async Task<Payment> CreatePayment(Payment payment)
    {
        var entry = await _appDBContext.Payment.AddAsync(payment);
        await _appDBContext.SaveChangesAsync();

        return entry.Entity;
    }
}