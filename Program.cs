global using TecnoCredito.Extensions;
using TecnoCredito.Models.System;

var builder = WebApplication.CreateBuilder(args);

// Add configuration for appsettings
builder
    .Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var appSettings = builder.Configuration.GetSection("App").Get<AppSettings>();
if (appSettings == null)
{
    throw new InvalidOperationException("No existe la sección App en la configuración");
}
builder.Services.AddSingleton(appSettings);

builder
    .Services.AddHttpContextAccessor()
    .ConfigureDatabase(builder.Configuration)
    .ConfigureIdentity()
    .ConfigureAutoMapper()
    .ConfigureJsonSetting(builder.Configuration)
    .ConfigureCors(appSettings)
    .AddDependencyInjection();

// Configure Controllers and API
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(ConfigureProgram.ConfigureSwaggerAuth);

// Configure JWT
builder.Services.ConfigureJwtSetting(builder.Configuration);

// Configure Authorization
builder.Services.AddAuthorization();

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

// Error handling middleware
app.UseExceptionHandler("/error");

await app.RunAsync();
