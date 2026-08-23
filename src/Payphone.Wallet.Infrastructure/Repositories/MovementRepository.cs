using Microsoft.EntityFrameworkCore;
using Payphone.Wallet.Application.Interfaces;
using Payphone.Wallet.Domain.Entities;
using Payphone.Wallet.Infrastructure.Persistence;

namespace Payphone.Wallet.Infrastructure.Repositories;

public sealed class MovementRepository : IMovementRepository
{
    private readonly WalletTransferDbContext _context;

    public MovementRepository(WalletTransferDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Movement>> GetByWalletIdAsync(int walletId, CancellationToken cancellationToken = default)
    {
        return await _context.Movements
            .Where(m => m.WalletId == walletId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}