using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Models.Money;

namespace TecnoCredito.Contexts.Configurations;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
  public void Configure(EntityTypeBuilder<PaymentMethod> builder)
  {
    builder.HasKey(e => e.Id);

    builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
    builder.Property(e => e.Description).HasMaxLength(200);
  }
}
