using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TecnoCredito.Models;
using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.Money;
using TecnoCredito.Models.Products;

namespace TecnoCredito.Contexts;

public class Context(DbContextOptions<Context> options) : IdentityDbContext<AppUser>(options)
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ConfigureModels();
  }

  #region Authentication
  public DbSet<AppUser> AppUsers { get; set; } = null!;
  #endregion

  #region Money
  public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
  #endregion

  #region Payment
  public DbSet<Category> Categories { get; set; } = null!;
  public DbSet<ContractProduct> ContractProducts { get; set; } = null!;
  public DbSet<Feature> Features { get; set; } = null!;
  public DbSet<Product> Products { get; set; } = null!;
  public DbSet<ProductFeature> ProductFeatures { get; set; } = null!;
  public DbSet<ProductImage> ProductImages { get; set; } = null!;
  #endregion

  #region Entities
  public DbSet<CustomerContract> CustomerContracts { get; set; } = null!;
  public DbSet<Installment> Installments { get; set; } = null!;
  public DbSet<Payment> Payments { get; set; } = null!;
  #endregion
}
