using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TecnoCredito.Models;

namespace TecnoCredito.Contexts.Configurations;

public class InstallmentConfiguration : IEntityTypeConfiguration<Installment>
{
    public void Configure(EntityTypeBuilder<Installment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
        builder.Property(e => e.Interest).HasColumnType("decimal(5,2)");

        builder
            .HasOne(d => d.Contract)
            .WithMany(p => p.Installments)
            .HasForeignKey(d => d.CustomerContractId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
