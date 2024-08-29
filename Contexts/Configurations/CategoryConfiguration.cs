using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Models.Products;

namespace TecnoCredito.Contexts.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
  public void Configure(EntityTypeBuilder<Category> builder)
  {
    builder.HasKey(e => e.Id);

    builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
    builder.Property(e => e.Description).HasMaxLength(200);

    builder.HasOne(d => d.ParentCategory)
      .WithMany(p => p.SubCategories)
      .HasForeignKey(d => d.ParentCategoryId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
