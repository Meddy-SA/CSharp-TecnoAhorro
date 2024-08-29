using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Models.Products;

namespace TecnoCredito.Contexts.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
  public void Configure(EntityTypeBuilder<Feature> builder)
  {
    builder.HasKey(e => e.Id);

    builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
    builder.Property(e => e.Description).HasMaxLength(200);
  }
}
