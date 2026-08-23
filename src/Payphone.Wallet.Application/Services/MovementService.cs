using Payphone.Wallet.Application.DTOs;
using Payphone.Wallet.Application.Interfaces;
using Payphone.Wallet.Domain.Exceptions;

namespace Payphone.Wallet.Application.Services;

public sealed class MovementService
{
    private readonly IMovementRepository _movementRepository;
    private readonly IWalletRepository _walletRepository;

    public MovementService(IMovementRepository movementRepository, IWalletRepository walletRepository)
    {
        _movementRepository = movementRepository;
        _walletRepository = walletRepository;
    }

    public async Task<IReadOnlyList<MovementDto>> GetHistoryByWalletIdAsync(int walletId, CancellationToken cancellationToken = default)
    {
        var walletExists = await _walletRepository.GetByIdAsync(walletId, cancellationToken)
            ?? throw new NotFoundException($"Wallet with id {walletId} was not found.");

        var movements = await _movementRepository.GetByWalletIdAsync(walletId, cancellationToken);

        return movements
            .Select(m => new MovementDto(m.Id, m.WalletId, m.Amount.Amount, m.Type.ToString(), m.CreatedAt))
            .ToList();
    }
}