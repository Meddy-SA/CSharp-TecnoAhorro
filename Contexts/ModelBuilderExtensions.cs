using Microsoft.EntityFrameworkCore;
using TecnoCredito.Contexts.Configurations;

namespace TecnoCredito.Contexts;

public static class ModelBuilderExtensions
{
  public static void ConfigureModels(this ModelBuilder builder)
  {
    builder.ApplyConfiguration(new CategoryConfiguration());
    builder.ApplyConfiguration(new ContractProductConfiguration());
    builder.ApplyConfiguration(new CustomerContractConfiguration());
    builder.ApplyConfiguration(new InstallmentConfiguration());
    builder.ApplyConfiguration(new FeatureConfiguration());
    builder.ApplyConfiguration(new PaymentConfiguration());
    builder.ApplyConfiguration(new PaymentMethodConfiguration());
    builder.ApplyConfiguration(new ProductsConfiguration());
    builder.ApplyConfiguration(new ProductFeatureConfiguration());
    builder.ApplyConfiguration(new SysMenuItemConfiguration());
    builder.ApplyConfiguration(new SysMenuCategoryConfiguration());
  }
}
