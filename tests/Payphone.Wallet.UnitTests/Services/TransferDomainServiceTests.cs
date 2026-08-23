using FluentAssertions;
using Payphone.Wallet.Domain.Enums;
using Payphone.Wallet.Domain.Exceptions;
using Payphone.Wallet.Domain.Services;
using Payphone.Wallet.Domain.ValueObjects;
using WalletEntity = Payphone.Wallet.Domain.Entities.Wallet;
using Xunit;

namespace Payphone.Wallet.UnitTests.Services;

public class TransferDomainServiceTests
{
    private readonly TransferDomainService _sut = new();

    private static WalletEntity CreateFundedWallet(
        decimal initialBalance,
        int id)
    {
        var documentId = id == 1
            ? "1234567890"
            : "0987654321";

        var name = id == 1
            ? "Source Owner"
            : "Destination Owner";

        var wallet = new WalletEntity(
            new DocumentId(documentId),
            name);

        // Asignamos un Id diferente a cada wallet para simular
        // las entidades después de haber sido guardadas en la BD.
        var idProperty = typeof(WalletEntity)
            .GetProperty(nameof(WalletEntity.Id));

        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(wallet, id);
        }
        else
        {
            var backingField = typeof(WalletEntity).GetField(
                "<Id>k__BackingField",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);

            if (backingField != null)
                backingField.SetValue(wallet, id);
        }

        if (initialBalance > 0)
            wallet.Credit(new Money(initialBalance));

        return wallet;
    }

    [Fact]
    public void Transfer_WithSufficientBalance_ShouldMoveAmountBetweenWallets()
    {
        var source = CreateFundedWallet(100, 1);
        var destination = CreateFundedWallet(0, 2);

        var (debit, credit) =
            _sut.Transfer(source, destination, new Money(30));

        source.Balance.Amount.Should().Be(70);
        destination.Balance.Amount.Should().Be(30);

        debit.Type.Should().Be(MovementType.Debit);
        credit.Type.Should().Be(MovementType.Credit);
    }

    [Fact]
    public void Transfer_WithInsufficientBalance_ShouldThrowAndLeaveDestinationUntouched()
    {
        var source = CreateFundedWallet(10, 1);
        var destination = CreateFundedWallet(0, 2);

        var act = () =>
            _sut.Transfer(source, destination, new Money(50));

        act.Should().Throw<WalletDomainException>()
            .Which.ErrorCode.Should()
            .Be(DomainErrorCode.InsufficientBalance);

        // Si el débito falla, el destino no debe recibir dinero.
        destination.Balance.Amount.Should().Be(0);
    }

    [Fact]
    public void Transfer_ToSameWallet_ShouldThrowDomainException()
    {
        var wallet = CreateFundedWallet(100, 1);

        var act = () =>
            _sut.Transfer(wallet, wallet, new Money(10));

        act.Should().Throw<WalletDomainException>()
            .Which.ErrorCode.Should()
            .Be(DomainErrorCode.SameWalletTransfer);
    }
}