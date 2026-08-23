using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payphone.Wallet.Domain.Entities;
using Payphone.Wallet.Domain.ValueObjects;

namespace Payphone.Wallet.Infrastructure.Persistence.Configurations;

public sealed class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder.ToTable("Movements");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();

        builder.Property(m => m.WalletId).IsRequired();

        builder.Property(m => m.Amount)
            .HasConversion(
                money => money.Amount,
                amount => new Money(amount))
            .HasColumnName("Amount")
            .HasColumnType("decimal(18,2)");

        builder.Property(m => m.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.CreatedAt).IsRequired();
    }
}