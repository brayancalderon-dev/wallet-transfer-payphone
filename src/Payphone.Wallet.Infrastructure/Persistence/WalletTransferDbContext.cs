using Microsoft.EntityFrameworkCore;
using WalletEntity = Payphone.Wallet.Domain.Entities.Wallet;
using Payphone.Wallet.Domain.Entities;

namespace Payphone.Wallet.Infrastructure.Persistence;

public sealed class WalletTransferDbContext : DbContext
{
    public WalletTransferDbContext(DbContextOptions<WalletTransferDbContext> options) : base(options) { }

    public DbSet<WalletEntity> Wallets => Set<WalletEntity>();
    public DbSet<Movement> Movements => Set<Movement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletTransferDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}