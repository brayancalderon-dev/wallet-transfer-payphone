namespace Payphone.Wallet.Application.DTOs;

public sealed record WalletDto(
    int Id,
    string DocumentId,
    string Name,
    decimal Balance,
    DateTime CreatedAt,
    DateTime UpdatedAt);