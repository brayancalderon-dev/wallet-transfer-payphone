using System.Text.RegularExpressions;
using Payphone.Wallet.Domain.Exceptions;

namespace Payphone.Wallet.Domain.ValueObjects;

public readonly partial record struct DocumentId
{
    public string Value { get; }

    public DocumentId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new WalletDomainException(
                DomainErrorCode.InvalidDocumentId,
                "DocumentId cannot be empty.");

        value = value.Trim();

        if (!DocumentIdRegex().IsMatch(value))
            throw new WalletDomainException(
                DomainErrorCode.InvalidDocumentId,
                "DocumentId must be a valid Ecuadorian cedula (10 digits) or RUC (13 digits).");

        Value = value;
    }

    [GeneratedRegex(@"^\d{10}(\d{3})?$")]
    private static partial Regex DocumentIdRegex();

    public override string ToString() => Value;
}