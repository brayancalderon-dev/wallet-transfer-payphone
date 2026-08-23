using Payphone.Wallet.Application.Interfaces;
using Payphone.Wallet.Infrastructure.Persistence;

namespace Payphone.Wallet.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly WalletTransferDbContext _context;

    public UnitOfWork(WalletTransferDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}