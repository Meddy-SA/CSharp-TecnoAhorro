using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Models;

namespace TecnoCredito.Contexts.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
  public void Configure(EntityTypeBuilder<Payment> builder)
  {
    builder.HasKey(e => e.Id);

    builder.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.PaymentProof).HasMaxLength(20);

    builder.HasOne(d => d.User)
      .WithMany(p => p.Payments)
      .HasForeignKey(d => d.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(d => d.CustomerContract)
      .WithMany(p => p.Payments)
      .HasForeignKey(d => d.ContractId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(d => d.PaymentMethod)
      .WithMany(p => p.Payments)
      .HasForeignKey(d => d.PaymentMethodId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
