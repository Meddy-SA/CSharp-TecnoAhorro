using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Models.Products;

namespace TecnoCredito.Contexts.Configurations;

public class ProductsConfiguration : IEntityTypeConfiguration<Product>
{
  public void Configure(EntityTypeBuilder<Product> builder)
  {
    builder.HasKey(e => e.Id);

    builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
    builder.Property(p => p.Description).HasMaxLength(200);
    builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
    builder.Property(p => p.Brand).HasMaxLength(50);
    builder.Property(p => p.Model).HasMaxLength(50);
    builder.Property(p => p.SKU).HasMaxLength(20).IsRequired();
    builder.Property(p => p.TechnicalSpecifications).HasMaxLength(30);

    builder.HasOne(d => d.Category)
      .WithMany(p => p.Products)
      .HasForeignKey(d => d.CategoryId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
