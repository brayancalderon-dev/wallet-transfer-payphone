namespace Payphone.Wallet.Application.DTOs;

public sealed record CreateWalletRequest(
    string DocumentId,
    string Name,
    decimal Balance);