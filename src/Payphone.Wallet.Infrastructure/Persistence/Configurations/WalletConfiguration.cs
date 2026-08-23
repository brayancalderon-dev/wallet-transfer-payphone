using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payphone.Wallet.Domain.ValueObjects;
using WalletEntity = Payphone.Wallet.Domain.Entities.Wallet;

namespace Payphone.Wallet.Infrastructure.Persistence.Configurations;

public sealed class WalletConfiguration : IEntityTypeConfiguration<WalletEntity>
{
    public void Configure(EntityTypeBuilder<WalletEntity> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .ValueGeneratedOnAdd();

        builder.Property(w => w.Balance)
            .HasConversion(
                money => money.Amount,
                amount => new Money(amount))
            .HasColumnName("Balance")
            .HasColumnType("decimal(18,2)");

        builder.Property(w => w.DocumentId)
            .HasConversion(
                docId => docId.Value,
                value => new DocumentId(value))
            .HasColumnName("DocumentId")
            .HasMaxLength(13)
            .IsRequired();

        builder.HasIndex(w => w.DocumentId).IsUnique();

        builder.Property(w => w.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.CreatedAt).IsRequired();
        builder.Property(w => w.UpdatedAt).IsRequired();

        builder.HasMany(w => w.Movements)
            .WithOne()
            .HasForeignKey(m => m.WalletId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(w => w.Movements)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(WalletEntity.Movements))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}