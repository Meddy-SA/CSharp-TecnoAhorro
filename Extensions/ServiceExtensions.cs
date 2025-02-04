using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TecnoCredito.Contexts;
using TecnoCredito.Middlewares;
using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.System;
using TecnoCredito.Services;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<Context>(
            options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );
                    }
                ),
            ServiceLifetime.Scoped
        );

        return services;
    }

    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<AppUser, AppRole>(options =>
            {
                // Password settings
                options.Password = new PasswordOptions
                {
                    RequireDigit = false, // Requerir un número
                    RequiredLength = 6, // ✓ Longitud mínima
                    RequireUppercase = false, // Requiere mayúsculas
                    RequireLowercase = false, // Requiere minusculas
                    RequireNonAlphanumeric = false, // Caracteres especiales
                    RequiredUniqueChars = 1, // Mínimo de caracteres únicos
                };

                // Lockout settings
                options.Lockout = new LockoutOptions
                {
                    DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5),
                    MaxFailedAccessAttempts = 5,
                    AllowedForNewUsers = true,
                };

                // User settings
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.SignIn = new SignInOptions
                {
                    RequireConfirmedEmail = false,
                    RequireConfirmedAccount = false,
                    RequireConfirmedPhoneNumber = false,
                };
            })
            .AddEntityFrameworkStores<Context>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<LocalizedIdentityErrorDescriber>();

        services.AddScoped<IUserTwoFactorTokenProvider<AppUser>, TwoFactorTokenProvider>();
        services.ConfigureApplicationCookie(ConfigureProgram.ConfigureAppCookie);

        return services;
    }

    public static IServiceCollection ConfigureJwtSetting(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var jwtSettings = new JwtSettings();
        configuration.GetSection("JWT").Bind(jwtSettings);

        var secretKey = System.Text.Encoding.UTF8.GetBytes(jwtSettings.Key);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ClockSkew = TimeSpan.Zero,
                };
            });

        services.AddScoped<IClaim, ClaimService>();

        return services;
    }

    public static IServiceCollection ConfigureJsonSetting(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 52428800;
        }); // 50 MB

        // Configure IIS Server Options
        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = configuration
                .GetSection("FileUpload:MaxFileSize")
                .Get<long>();
        });
        // Configre Kestrel
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = configuration
                .GetSection("FileUpload:MaxFileSize")
                .Get<long?>();
        });
        // Configure Form Options
        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = configuration
                .GetSection("FileUpload:MaxFileSize")
                .Get<long>();
        });
        return services;
    }

    public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
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

    public static IServiceCollection ConfigureCors(
        this IServiceCollection services,
        AppSettings appSettings
    )
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "cors",
                builderPolicy =>
                {
                    builderPolicy
                        .WithOrigins(
                            appSettings.Cors.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .SetPreflightMaxAge(TimeSpan.FromSeconds(2520));
                }
            );
        });

        return services;
    }

    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<ICategoryHandle, CategoryService>();
        services.AddTransient<IEmailSender, EmailService>();
        services.AddScoped<IEnumeratorHandle, EnumeratorService>();
        services.AddTransient<IProductHandle, ProductService>();
        services.AddTransient<ISysMenu, SysMenuService>();
        services.AddTransient<IUserHandle, UserHandleService>();

        return services;
    }
}
