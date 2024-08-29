using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Models.Products;

namespace TecnoCredito.Contexts.Configurations;

public class ContractProductConfiguration : IEntityTypeConfiguration<ContractProduct>
{
  public void Configure(EntityTypeBuilder<ContractProduct> builder)
  {
    builder.HasKey(e => new { e.CustomerContractId, e.ProductId });

    builder.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

    builder.HasOne(d => d.CustomerContract)
      .WithMany(p => p.ContractProducts)
      .HasForeignKey(d => d.CustomerContractId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(d => d.Product)
      .WithMany(p => p.ContractProducts)
      .HasForeignKey(d => d.ProductId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
