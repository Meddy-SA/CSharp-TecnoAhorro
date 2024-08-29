using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Models.Products;

namespace TecnoCredito.Contexts.Configurations;

public class ProductFeatureConfiguration : IEntityTypeConfiguration<ProductFeature>
{
  public void Configure(EntityTypeBuilder<ProductFeature> builder)
  {
    builder.HasKey(e => new { e.ProductId, e.FeatureId });

    builder.HasOne(d => d.Product)
      .WithMany(p => p.ProductFeatures)
      .HasForeignKey(d => d.ProductId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(d => d.Feature)
      .WithMany(p => p.ProductFeatures)
      .HasForeignKey(d => d.FeatureId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
