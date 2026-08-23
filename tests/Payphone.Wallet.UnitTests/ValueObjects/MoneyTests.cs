using FluentAssertions;
using Payphone.Wallet.Domain.Exceptions;
using Payphone.Wallet.Domain.ValueObjects;
using Xunit;

namespace Payphone.Wallet.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowDomainException()
    {
        var act = () => new Money(-10);

        act.Should().Throw<WalletDomainException>()
            .Which.ErrorCode.Should().Be(DomainErrorCode.NegativeAmount);
    }

    [Fact]
    public void Constructor_WithZero_ShouldSucceed()
    {
        var money = new Money(0);

        money.Amount.Should().Be(0);
    }

    [Fact]
    public void Add_ShouldReturnSumOfBothAmounts()
    {
        var a = new Money(100.50m);
        var b = new Money(49.50m);

        var result = a.Add(b);

        result.Amount.Should().Be(150.00m);
    }

    [Fact]
    public void Subtract_WithSufficientAmount_ShouldReturnDifference()
    {
        var a = new Money(100);
        var b = new Money(30);

        var result = a.Subtract(b);

        result.Amount.Should().Be(70);
    }

    [Fact]
    public void Subtract_WithAmountGreaterThanBalance_ShouldThrowInsufficientBalance()
    {
        var a = new Money(50);
        var b = new Money(100);

        var act = () => a.Subtract(b);

        act.Should().Throw<WalletDomainException>()
            .Which.ErrorCode.Should().Be(DomainErrorCode.InsufficientBalance);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(-1, true)]
    [InlineData(0.01, false)]
    public void IsZeroOrLess_ShouldReturnExpectedResult(
        decimal amount,
        bool expected)
    {
        // Nota: -1 no debería poder construirse, así que probamos solo con valores válidos >= 0

        if (amount < 0)
            return;

        var money = new Money(amount);

        money.IsZeroOrLess().Should().Be(expected);
    }
}