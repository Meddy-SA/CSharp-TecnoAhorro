global using TecnoCredito.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TecnoCredito.Contexts;
using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.System;
using TecnoCredito.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

// Add configuration for appsettings
builder.Configuration
  .SetBasePath(builder.Environment.ContentRootPath)
  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
  .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
  .AddEnvironmentVariables();


var appSettings = builder.Configuration.GetSection("App").Get<AppSettings>();
if (appSettings != null)
{
  builder.Services.AddSingleton(appSettings);
}
else
{
  throw new InvalidOperationException("No existe la sección App en la configuración");
}

#region Configure CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("cors",
    builderPolicy =>
    {
      builderPolicy.WithOrigins(appSettings.Cors.Split(",", StringSplitOptions.RemoveEmptyEntries))
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
      .SetPreflightMaxAge(TimeSpan.FromSeconds(2520));
    });
});
#endregion

// For JSON
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 52428800; }); // 50 MB

#region Jwt & SQL Server
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JWT").Bind(jwtSettings);
// Add configuration to Sql Server
builder.Services.AddDbContext<Context>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"))
);
#endregion

#region Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
  options.SignIn.RequireConfirmedAccount = true;
}).AddEntityFrameworkStores<Context>()
  .AddRoles<IdentityRole>()
  .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserTwoFactorTokenProvider<AppUser>, TwoFactorTokenProvider>();
builder.Services.Configure<IdentityOptions>(ConfigureProgram.ConfigureIdentityOptions);
builder.Services.ConfigureApplicationCookie(ConfigureProgram.ConfigureAppCookie);
#endregion

#region Dependency Inject Services
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDependencyInjection();
#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(ConfigureProgram.ConfigureSwaggerAuth);

#region Add Authentication
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Add Jwt Bearer
.AddJwtBearer(options => ConfigureProgram.ConfigureJwtBearer(options, jwtSettings.Audience, jwtSettings.Issuer, jwtSettings.Key));

builder.Services.AddAuthorization();

#endregion

#endregion Configuration

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Configure Upload
// Configure IIS Server Options
builder.Services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = builder.Configuration.GetSection("FileUpload:MaxFileSize").Get<long>(); });
// Configre Kestrel 
builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = builder.Configuration.GetSection("FileUpload:MaxFileSize").Get<long?>(); });
// Configure Form Options
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = builder.Configuration.GetSection("FileUpload:MaxFileSize").Get<long>(); });
#endregion Configure Upload

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

// UNDONE: Voy a comentar la siguiente linea para pruebas.
// app.UseHttpsRedirection();

app.UseCors("cors");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
