using Payphone.Wallet.Application.DTOs;
using Payphone.Wallet.Application.Interfaces;
using Payphone.Wallet.Domain.Exceptions;
using Payphone.Wallet.Domain.ValueObjects;
using WalletTransfer.Application.DTOs;
using WalletEntity = Payphone.Wallet.Domain.Entities.Wallet;

namespace Payphone.Wallet.Application.Services;

public sealed class WalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WalletService(
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WalletDto> CreateWalletAsync(
        CreateWalletRequest request,
        CancellationToken cancellationToken = default)
    {
        var documentId = new DocumentId(request.DocumentId);

        var alreadyExists = await _walletRepository.ExistsByDocumentIdAsync(
            documentId.Value,
            cancellationToken);

        if (alreadyExists)
        {
            throw new WalletDomainException(
                DomainErrorCode.InvalidDocumentId,
                "A wallet already exists for this DocumentId.");
        }

        var wallet = new WalletEntity(
            documentId,
            request.Name,
            new Money(request.Balance));

        await _walletRepository.AddAsync(
            wallet,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToDto(wallet);
    }

    public async Task<WalletDto> GetWalletByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository.GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new NotFoundException(
                $"Wallet with id {id} was not found.");

        return MapToDto(wallet);
    }

    public async Task<WalletDto> UpdateWalletAsync(
        int id,
        UpdateWalletRequest request,
        CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository.GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new NotFoundException(
                $"Wallet with id {id} was not found.");

        wallet.UpdateName(request.Name);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToDto(wallet);
    }

    public async Task DeleteWalletAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository.GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new NotFoundException(
                $"Wallet with id {id} was not found.");

        wallet.EnsureCanBeDeleted();

        _walletRepository.Remove(wallet);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    private static WalletDto MapToDto(
        WalletEntity wallet) => new(
            wallet.Id,
            wallet.DocumentId.Value,
            wallet.Name,
            wallet.Balance.Amount,
            wallet.CreatedAt,
            wallet.UpdatedAt);
}