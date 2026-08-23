namespace Payphone.Wallet.Application.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}