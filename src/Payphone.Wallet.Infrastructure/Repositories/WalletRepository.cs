using Microsoft.EntityFrameworkCore;
using Payphone.Wallet.Application.Interfaces;
using WalletEntity = Payphone.Wallet.Domain.Entities.Wallet;
using Payphone.Wallet.Infrastructure.Persistence;

namespace Payphone.Wallet.Infrastructure.Repositories;

public sealed class WalletRepository : IWalletRepository
{
    private readonly WalletTransferDbContext _context;

    public WalletRepository(WalletTransferDbContext context)
    {
        _context = context;
    }

    public async Task<WalletEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Wallets
            .Include(w => w.Movements)
            .FirstOrDefaultAsync(
                w => w.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByDocumentIdAsync(
        string documentId,
        CancellationToken cancellationToken = default)
    {
        var wallets = await _context.Wallets
            .Select(w => w.DocumentId)
            .ToListAsync(cancellationToken);

        return wallets.Any(d => d.Value == documentId);
    }

    public async Task AddAsync(
        WalletEntity wallet,
        CancellationToken cancellationToken = default)
    {
        await _context.Wallets.AddAsync(
            wallet,
            cancellationToken);
    }

    public void Remove(WalletEntity wallet)
    {
        _context.Wallets.Remove(wallet);
    }
}