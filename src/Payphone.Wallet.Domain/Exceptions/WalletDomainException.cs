namespace Payphone.Wallet.Domain.Exceptions;

public enum DomainErrorCode
{
    NegativeAmount,
    InvalidDocumentId,
    InsufficientBalance,
    InvalidName,
    SameWalletTransfer,
    CannotDeleteWalletWithBalance
}

public sealed class WalletDomainException : Exception
{
    public DomainErrorCode ErrorCode { get; }

    public WalletDomainException(DomainErrorCode errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}