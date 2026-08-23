using Payphone.Wallet.Domain.Exceptions;

namespace Payphone.Wallet.Domain.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new WalletDomainException(
                DomainErrorCode.NegativeAmount,
                "Amount cannot be negative.");

        Amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
    }

    public static Money Zero => new(0);

    public Money Add(Money other) => new(Amount + other.Amount);

    public Money Subtract(Money other)
    {
        if (other.Amount > Amount)
            throw new WalletDomainException(
                DomainErrorCode.InsufficientBalance,
                "Insufficient balance for this operation.");

        return new Money(Amount - other.Amount);
    }

    public bool IsZeroOrLess() => Amount <= 0;

    public override string ToString() => Amount.ToString("F2");
}