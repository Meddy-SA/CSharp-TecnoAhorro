using Microsoft.OpenApi.Models;

namespace TecnoCredito.Extensions;

public static class ConfigureProgram
{
    public static void ConfigureAppCookie(
        Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions options
    )
    {
        options.Cookie.SameSite = SameSiteMode.None; // Permite que la cookie se envíe en solicitudes cross-origin
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Las cookies solo se enviarán a través de HTTPS
        // options.Cookie.HttpOnly = true; // Opcional, hace que la cookie no sea accesible desde JavaScript
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // La cookie expira después de 20 minutos
        options.SlidingExpiration = true; // La expiración se renueva con actividad
    }

    public static void ConfigureSwaggerAuth(
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options
    )
    {
        options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            }
        );
        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            }
        );
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Meddy.API", Version = "1.0.1" });
    }
}
