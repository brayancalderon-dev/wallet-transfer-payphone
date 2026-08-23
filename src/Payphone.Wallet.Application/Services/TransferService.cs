using Payphone.Wallet.Application.DTOs;
using Payphone.Wallet.Application.Interfaces;
using Payphone.Wallet.Domain.Exceptions;
using Payphone.Wallet.Domain.Services;
using Payphone.Wallet.Domain.ValueObjects;

namespace Payphone.Wallet.Application.Services;

public sealed class TransferService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TransferDomainService _transferDomainService;

    public TransferService(
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
        _transferDomainService = new TransferDomainService();
    }

    public async Task TransferAsync(
        TransferRequest request,
        CancellationToken cancellationToken = default)
    {
        var source = await _walletRepository.GetByIdAsync(
            request.SourceWalletId,
            cancellationToken)
            ?? throw new NotFoundException(
                $"Source wallet with id {request.SourceWalletId} was not found.");

        var destination = await _walletRepository.GetByIdAsync(
            request.DestinationWalletId,
            cancellationToken)
            ?? throw new NotFoundException(
                $"Destination wallet with id {request.DestinationWalletId} was not found.");

        var amount = new Money(request.Amount);

        _transferDomainService.Transfer(source, destination, amount);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}