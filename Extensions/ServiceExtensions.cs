using TecnoCredito.Services;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Extensions;

public static class ServiceExtensions
{
  public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
  {
    services.AddTransient<IClaim, ClaimService>();
    services.AddScoped<ICategoryHandle, CategoryService>();
    services.AddTransient<IEmailSender, EmailService>();
    services.AddScoped<IEnumeratorHandle, EnumeratorService>();
    services.AddTransient<IProductHandle, ProductService>();
    services.AddTransient<ISysMenu, SysMenuService>();
    services.AddTransient<IUserHandle, UserHandleService>();

    return services;
  }
}
