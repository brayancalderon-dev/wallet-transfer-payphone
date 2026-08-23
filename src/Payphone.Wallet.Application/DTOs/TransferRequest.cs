namespace Payphone.Wallet.Application.DTOs;

public sealed record TransferRequest(
    int SourceWalletId,
    int DestinationWalletId,
    decimal Amount);