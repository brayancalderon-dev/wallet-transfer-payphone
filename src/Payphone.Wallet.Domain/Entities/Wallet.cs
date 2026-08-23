using Payphone.Wallet.Domain.Exceptions;
using Payphone.Wallet.Domain.ValueObjects;

namespace Payphone.Wallet.Domain.Entities;

public sealed class Wallet
{
    private readonly List<Movement> _movements = new();

    public int Id { get; private set; }

    public DocumentId DocumentId { get; private set; }

    public string Name { get; private set; } = default!;

    public Money Balance { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<Movement> Movements =>
        _movements.AsReadOnly();

    private Wallet()
    {
        // Constructor utilizado por EF Core
    }

    public Wallet(
        DocumentId documentId,
        string name,
        Money initialBalance)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new WalletDomainException(
                DomainErrorCode.InvalidName,
                "Name cannot be empty.");
        }

        DocumentId = documentId;
        Name = name.Trim();
        Balance = initialBalance;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Movement Debit(Money amount)
    {
        EnsurePositiveAmount(amount);

        Balance = Balance.Subtract(amount);

        var movement = Movement.CreateDebit(
            Id,
            amount);

        _movements.Add(movement);

        UpdatedAt = DateTime.UtcNow;

        return movement;
    }

    public Movement Credit(Money amount)
    {
        EnsurePositiveAmount(amount);

        Balance = Balance.Add(amount);

        var movement = Movement.CreateCredit(
            Id,
            amount);

        _movements.Add(movement);

        UpdatedAt = DateTime.UtcNow;

        return movement;
    }

    private static void EnsurePositiveAmount(Money amount)
    {
        if (amount.IsZeroOrLess())
        {
            throw new WalletDomainException(
                DomainErrorCode.NegativeAmount,
                "Amount must be greater than zero.");
        }
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new WalletDomainException(
                DomainErrorCode.InvalidName,
                "Name cannot be empty.");
        }

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnsureCanBeDeleted()
    {
        if (!Balance.IsZeroOrLess())
        {
            throw new WalletDomainException(
                DomainErrorCode.CannotDeleteWalletWithBalance,
                "Cannot delete a wallet with a positive balance.");
        }
    }
}