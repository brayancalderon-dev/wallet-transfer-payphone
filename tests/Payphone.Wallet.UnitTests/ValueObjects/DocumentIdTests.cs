using FluentAssertions;
using Payphone.Wallet.Domain.Exceptions;
using Payphone.Wallet.Domain.ValueObjects;
using Xunit;

namespace Payphone.Wallet.UnitTests.ValueObjects;

public class DocumentIdTests
{
    [Theory]
    [InlineData("1234567890")]      // cédula: 10 dígitos
    [InlineData("1234567890123")]   // RUC: 13 dígitos
    public void Constructor_WithValidFormat_ShouldSucceed(string value)
    {
        var documentId = new DocumentId(value);

        documentId.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("abcdefghij")]
    [InlineData("12345678901")]
    public void Constructor_WithInvalidFormat_ShouldThrowDomainException(
        string value)
    {
        var act = () => new DocumentId(value);

        act.Should().Throw<WalletDomainException>()
            .Which.ErrorCode.Should()
            .Be(DomainErrorCode.InvalidDocumentId);
    }
}