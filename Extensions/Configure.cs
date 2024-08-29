using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace TecnoCredito.Extensions;

public static class ConfigureProgram
{

  public static void ConfigureIdentityOptions(IdentityOptions options)
  {
    // SignIn settings
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
  }

  public static void ConfigureAppCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions options)
  {
    options.Cookie.SameSite = SameSiteMode.None; // Permite que la cookie se envíe en solicitudes cross-origin
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Las cookies solo se enviarán a través de HTTPS
    // options.Cookie.HttpOnly = true; // Opcional, hace que la cookie no sea accesible desde JavaScript
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // La cookie expira después de 20 minutos
    options.SlidingExpiration = true; // La expiración se renueva con actividad
  }

  public static void ConfigureJwtBearer(JwtBearerOptions options, string audience, string issuer, string key)
  {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidAudience = audience,
      ValidIssuer = issuer,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
  }

  public static void ConfigureSwaggerAuth(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
  {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
      Name = "Authorization",
      Type = SecuritySchemeType.ApiKey,
      Scheme = "Bearer",
      BearerFormat = "JWT",
      In = ParameterLocation.Header,
      Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
      {
        new OpenApiSecurityScheme {
          Reference = new OpenApiReference {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
          }
        },
        Array.Empty<string>()
      }
    });
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Meddy.API", Version = "1.0.1" });
  }
}
