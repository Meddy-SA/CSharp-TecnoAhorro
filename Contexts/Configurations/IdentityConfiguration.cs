using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TecnoCredito.Models.Authentication;

namespace TecnoCredito.Contexts.Configurations;

public static class IdentityConfiguration
{
    public static void ConfigureIdentityTables(this ModelBuilder builder)
    {
        // Configuración específica para AppUser
        builder.Entity<AppUser>(entity =>
        {
            entity.ToTable("AspNetUsers");

            // Valores por defecto para propiedades nullable
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            // Configuración de propiedades string
            entity.Property(e => e.FirstName).HasMaxLength(60).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(60).IsRequired();
            entity.Property(e => e.MiddleName).HasMaxLength(60);
            entity.Property(e => e.SecondSurname).HasMaxLength(60);

            // Índices
            entity.HasIndex(e => e.NormalizedEmail).HasDatabaseName("EmailIndex");
            entity.HasIndex(e => e.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
        });

        // Configuración para PreRegister
        builder.Entity<PreRegister>(entity =>
        {
            entity.ToTable("PreRegisters");

            // Primary Key (using Email as primary key since it's unique per pre-registration)
            entity.HasKey(e => e.Email);

            // Configuración de propiedades string
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(180).IsRequired();
            entity.Property(e => e.Country).HasMaxLength(2).IsRequired().HasDefaultValue("ar");

            // Valores por defecto para propiedades boolean
            entity.Property(e => e.TermsAccepted).HasDefaultValue(false);
            entity.Property(e => e.EmailUpdated).HasDefaultValue(false);

            // Configuración de la relación con AppUser
            entity.Property(e => e.AppUserId).HasMaxLength(450);
            entity
                .HasOne(p => p.AppUser)
                .WithOne()
                .HasForeignKey<PreRegister>(p => p.AppUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índice en Email
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configuración de las tablas de Identity
        builder.Entity<AppRole>(entity =>
        {
            entity.ToTable("AspNetRoles");

            entity.Property(e => e.Id).HasMaxLength(450);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);

            // Primary Key
            entity.HasKey(e => e.Id);

            // Índice único en NormalizedName
            entity
                .HasIndex(e => e.NormalizedName)
                .HasDatabaseName("RoleNameIndex")
                .IsUnique()
                .HasFilter("[NormalizedName] IS NOT NULL");
        });

        builder.Entity<AppUserRole>(entity =>
        {
            entity.ToTable("AspNetUserRoles");

            // Clave primaria compuesta
            entity.HasKey(e => new { e.UserId, e.RoleId });

            // Propiedades
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.RoleId).HasMaxLength(450);

            // Relaciones
            entity
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            entity
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("AspNetUserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("AspNetUserLogins");
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("AspNetRoleClaims");
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("AspNetUserTokens");
        });
    }
}
