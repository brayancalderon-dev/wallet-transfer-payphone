using FluentAssertions;
using Payphone.Wallet.Domain.Exceptions;
using Payphone.Wallet.Domain.ValueObjects;
using Payphone.Wallet.Domain.Enums;
using WalletEntity = Payphone.Wallet.Domain.Entities.Wallet;
using Xunit;

namespace Payphone.Wallet.UnitTests.Entities;

public class WalletTests
{
    private static WalletEntity CreateWallet() =>
        new(
            new DocumentId("1234567890"),
            "Juan Perez",
            Money.Zero);

    [Fact]
    public void Constructor_ShouldStartWithZeroBalance()
    {
        var wallet = CreateWallet();

        wallet.Balance.Amount.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowDomainException()
    {
        var act = () =>
            new WalletEntity(
                new DocumentId("1234567890"),
                "  ",
                Money.Zero);

        act.Should().Throw<WalletDomainException>()
            .Which.ErrorCode.Should()
            .Be(DomainErrorCode.InvalidName);
    }

    [Fact]
    public void Credit_ShouldIncreaseBalanceAndCreateMovement()
    {
        var wallet = CreateWallet();

        wallet.Credit(new Money(100));

        wallet.Balance.Amount.Should().Be(100);

        wallet.Movements.Should()
            .ContainSingle(m => m.Type == MovementType.Credit);
    }

    [Fact]
    public void Debit_WithSufficientBalance_ShouldDecreaseBalanceAndCreateMovement()
    {
        var wallet = CreateWallet();

        wallet.Credit(new Money(100));

        wallet.Debit(new Money(40));

        wallet.Balance.Amount.Should().Be(60);

        wallet.Movements.Should()
            .ContainSingle(m => m.Type == MovementType.Debit);
    }

    [Fact]
    public void Debit_WithInsufficientBalance_ShouldThrowDomainException()
    {
        var wallet = CreateWallet();

        var act = () => wallet.Debit(new Money(50));

        act.Should().Throw<WalletDomainException>()
            .Which.ErrorCode.Should()
            .Be(DomainErrorCode.InsufficientBalance);
    }

    [Fact]
    public void Debit_WithZeroAmount_ShouldThrowDomainException()
    {
        var wallet = CreateWallet();

        wallet.Credit(new Money(100));

        var act = () => wallet.Debit(new Money(0));

        act.Should().Throw<WalletDomainException>()
            .Which.ErrorCode.Should()
            .Be(DomainErrorCode.NegativeAmount);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldChangeNameAndUpdatedAt()
    {
        var wallet = CreateWallet();
        var originalUpdatedAt = wallet.UpdatedAt;

        wallet.UpdateName("Nuevo Nombre");

        wallet.Name.Should().Be("Nuevo Nombre");
        wallet.UpdatedAt.Should().BeOnOrAfter(originalUpdatedAt);
    }

    [Fact]
    public void EnsureCanBeDeleted_WithPositiveBalance_ShouldThrowDomainException()
    {
        var wallet = CreateWallet();

        wallet.Credit(new Money(10));

        var act = () => wallet.EnsureCanBeDeleted();

        act.Should().Throw<WalletDomainException>()
            .Which.ErrorCode.Should()
            .Be(DomainErrorCode.CannotDeleteWalletWithBalance);
    }

    [Fact]
    public void EnsureCanBeDeleted_WithZeroBalance_ShouldNotThrow()
    {
        var wallet = CreateWallet();

        var act = () => wallet.EnsureCanBeDeleted();

        act.Should().NotThrow();
    }
}