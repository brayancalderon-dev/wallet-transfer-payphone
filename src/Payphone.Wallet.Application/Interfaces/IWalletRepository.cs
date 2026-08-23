using Payphone.Wallet.Domain.Entities;

namespace Payphone.Wallet.Application.Interfaces;

public interface IWalletRepository
{
    Task<Domain.Entities.Wallet?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByDocumentIdAsync(
        string documentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Domain.Entities.Wallet wallet,
        CancellationToken cancellationToken = default);

    void Remove(Domain.Entities.Wallet wallet);
}