using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TecnoCredito.Contexts;
using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.System;
using TecnoCredito.Services;
using TecnoCredito.Services.Interfaces;
using TecnoCredito.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

#region Configure CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowSpecificOrigin",
    builderPolicy =>
    {
      builderPolicy.WithOrigins("http://localhost:5173", "https://meddy.myiphost.com")
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
builder.Services.AddTransient<IEmailSender, EmailService>();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
