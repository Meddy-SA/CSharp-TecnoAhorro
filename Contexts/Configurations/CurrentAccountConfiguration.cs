using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Models;

namespace TecnoCredito.Contexts.Configurations;

public class CurrentAccountConfiguration : IEntityTypeConfiguration<CurrentAccount>
{
  public void Configure(EntityTypeBuilder<CurrentAccount> builder)
  {
    builder.ToTable("CurrentAccounts");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.UserId)
        .HasMaxLength(450)
        .IsRequired();

    builder.Property(x => x.Date)
        .IsRequired();

    builder.Property(x => x.Type)
        .IsRequired();

    builder.Property(x => x.Amount)
        .HasColumnType("decimal(18,2)")
        .IsRequired();

    builder.Property(x => x.Balance)
        .HasColumnType("decimal(18,2)")
        .IsRequired();

    builder.Property(x => x.Description)
        .HasMaxLength(500);

    builder.Property(x => x.Status)
        .IsRequired();

    // Relación con productos a través de la tabla intermedia
    builder.HasMany(x => x.CurrentAccountProducts)
        .WithOne(x => x.CurrentAccount)
        .HasForeignKey(x => x.CurrentAccountId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}

public class CurrentAccountProductConfiguration : IEntityTypeConfiguration<CurrentAccountProduct>
{
  public void Configure(EntityTypeBuilder<CurrentAccountProduct> builder)
  {
    builder.ToTable("CurrentAccountProducts");

    builder.HasKey(x => new { x.CurrentAccountId, x.ProductId });

    builder.HasOne(x => x.CurrentAccount)
        .WithMany(x => x.CurrentAccountProducts)
        .HasForeignKey(x => x.CurrentAccountId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(x => x.Product)
        .WithMany()
        .HasForeignKey(x => x.ProductId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}

public class CreditLimitConfiguration : IEntityTypeConfiguration<CreditLimit>
{
  public void Configure(EntityTypeBuilder<CreditLimit> builder)
  {
    builder.ToTable("CreditLimits");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.UserId)
        .HasMaxLength(450)
        .IsRequired();

    builder.Property(x => x.Limit)
        .HasColumnType("decimal(18,2)")
        .IsRequired();

    builder.Property(x => x.Available)
        .HasColumnType("decimal(18,2)")
        .IsRequired();

    builder.Property(x => x.Status)
        .IsRequired();
  }
}
