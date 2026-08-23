using Payphone.Wallet.Domain.Entities;
using Payphone.Wallet.Domain.Exceptions;
using Payphone.Wallet.Domain.ValueObjects;

namespace Payphone.Wallet.Domain.Services;

public sealed class TransferDomainService
{
    public (Movement Debit, Movement Credit) Transfer(
        Entities.Wallet source,
        Entities.Wallet destination,
        Money amount)
    {
        if (source.Id == destination.Id)
            throw new WalletDomainException(
                DomainErrorCode.SameWalletTransfer,
                "Cannot transfer to the same wallet.");

        var debit = source.Debit(amount);
        var credit = destination.Credit(amount);

        return (debit, credit);
    }
}