using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Models;

namespace TecnoCredito.Contexts.Configurations;

public class CustomerContractConfiguration : IEntityTypeConfiguration<CustomerContract>
{
  public void Configure(EntityTypeBuilder<CustomerContract> builder)
  {
    builder.HasKey(e => e.Id);

    builder.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.InstallmentAmount).HasColumnType("decimal(18, 2)");
    builder.Property(e => e.InterestRate).HasColumnType("decimal(5, 2)");

    builder.HasOne(d => d.User)
      .WithMany(p => p.CustomerContracts)
      .HasForeignKey(d => d.UserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
