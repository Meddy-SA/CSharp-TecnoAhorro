using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Helpers;
using TecnoCredito.Models.System;

namespace TecnoCredito.Contexts.Configurations;

public class SysMenuItemConfiguration : IEntityTypeConfiguration<SysMenuItem>
{
    public void Configure(EntityTypeBuilder<SysMenuItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).HasMaxLength(255).IsRequired();
        builder.Property(e => e.Icon).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Style).HasMaxLength(50);
        builder.Property(e => e.Command).HasMaxLength(255);
        builder.Property(e => e.Badge).HasMaxLength(3);
        builder.Property(e => e.Order).IsRequired();

        // Configuración para el campo Roles como JSON
        builder
            .Property(e => e.Roles)
            .HasConversion(new IntListToStringConverter())
            .HasColumnType("varchar(100)")
            .IsRequired(false)
            .Metadata.SetValueComparer(IntListToStringConverter.GetComparer());

        // Configuración de la relación con SysMenuCategory
        builder
            .HasOne(e => e.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(e => e.SysMenuCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de la relación jerárquica (self-referencing)
        builder
            .HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(e => e.SysMenuItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SysMenuCategoryConfiguration : IEntityTypeConfiguration<SysMenuCategory>
{
    public void Configure(EntityTypeBuilder<SysMenuCategory> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(255).IsRequired();

        // Configuración para el campo Roles como JSON usando Newtonsoft.Json
        builder
            .Property(e => e.Roles)
            .HasConversion(
                v => string.Join(',', v ?? new List<int>()),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
            )
            .HasColumnType("varchar(100)")
            .Metadata.SetValueComparer(IntListToStringConverter.GetComparer());

        // Configuración de la relación con SysMenuItem
        builder
            .HasMany(e => e.Items)
            .WithOne(i => i.Category)
            .HasForeignKey(i => i.SysMenuCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
