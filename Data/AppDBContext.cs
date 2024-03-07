using Microsoft.EntityFrameworkCore;
using APIx.Models;

namespace APIx.Data;

public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
{
	public DbSet<User> User { get; set; } = null!;

	public DbSet<PaymentProvider> PaymentProvider { get; set; } = null!;

	public DbSet<PaymentProviderAccount> PaymentProviderAccount { get; set; } = null!;

	public DbSet<PixKey> PixKey { get; set; } = null!;


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<PaymentProvider>()
			.Property(p => p.CreatedAt)
			.HasDefaultValueSql("NOW()");

		modelBuilder.Entity<PaymentProvider>()
            .Property(p => p.UpdatedAt)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnUpdate();

		modelBuilder.Entity<PaymentProviderAccount>()
			.Property(p => p.CreatedAt)
			.HasDefaultValueSql("NOW()");

		modelBuilder.Entity<PaymentProviderAccount>()
            .Property(p => p.UpdatedAt)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnUpdate();

		modelBuilder.Entity<PixKey>()
			.Property(p => p.CreatedAt)
			.HasDefaultValueSql("NOW()");

		modelBuilder.Entity<PixKey>()
            .Property(p => p.UpdatedAt)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnUpdate();

		modelBuilder.Entity<User>()
			.Property(p => p.CreatedAt)
			.HasDefaultValueSql("NOW()");

		modelBuilder.Entity<User>()
            .Property(p => p.UpdatedAt)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnUpdate();
	}
}