using Payphone.Wallet.Domain.Enums;
using Payphone.Wallet.Domain.ValueObjects;

namespace Payphone.Wallet.Domain.Entities;

public sealed class Movement
{
    public int Id { get; private set; }
    public int WalletId { get; private set; }
    public Money Amount { get; private set; }
    public MovementType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Movement() { }

    private Movement(int walletId, Money amount, MovementType type)
    {
        WalletId = walletId;
        Amount = amount;
        Type = type;
        CreatedAt = DateTime.UtcNow;
    }

    public static Movement CreateDebit(int walletId, Money amount) =>
        new(walletId, amount, MovementType.Debit);

    public static Movement CreateCredit(int walletId, Money amount) =>
        new(walletId, amount, MovementType.Credit);
}