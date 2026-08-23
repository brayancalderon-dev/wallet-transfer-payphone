namespace Payphone.Wallet.Application.DTOs;

public sealed record MovementDto(
    int Id,
    int WalletId,
    decimal Amount,
    string Type,
    DateTime CreatedAt);