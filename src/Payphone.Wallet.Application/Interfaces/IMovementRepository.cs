using Payphone.Wallet.Domain.Entities;

namespace Payphone.Wallet.Application.Interfaces;

public interface IMovementRepository
{
    Task<IReadOnlyList<Movement>> GetByWalletIdAsync(
        int walletId,
        CancellationToken cancellationToken = default);
}