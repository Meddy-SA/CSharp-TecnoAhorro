using Microsoft.AspNetCore.Identity;
using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.DTOs;

namespace TecnoCredito.Middlewares;

public class DbInitializer
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<DbInitializer> _logger;

    private const string DefaultPassword = "Password123!";
    private static bool _isInitialized;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public DbInitializer(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ILogger<DbInitializer> logger
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<ResponseDTO<bool>> InitializeUsersAsync()
    {
        // Primera verificación rápida sin bloqueo
        if (_isInitialized)
        {
            return ResponseDTO<bool>.Success(true);
        }

        try
        {
            // Esperamos a obtener el semáforo
            await _semaphore.WaitAsync();

            // Segunda verificación una vez que tenemos el bloqueo
            if (_isInitialized)
            {
                return ResponseDTO<bool>.Success(true);
            }

            var users = new List<(AppUser User, string Role)>
            {
                (CreateUserModel("leonardoillanez@meddyai.com", "Super", "Admin"), "SuperAdmin"),
            };

            foreach (var (user, role) in users)
            {
                await EnsureRolesAsync(role);
                await CreateUserIfNotExistsAsync(user, role);
            }

            _isInitialized = true;
            return ResponseDTO<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar usuarios de prueba");
            return ResponseDTO<bool>.Failure("Error al inicializar usuarios de prueba");
        }
        finally
        {
            // Muy importante: siempre liberamos el semáforo
            _semaphore.Release();
        }
    }

    private AppUser CreateUserModel(string email, string firstName, string lastName)
    {
        return new AppUser
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            PhoneNumber = "+505555555",
        };
    }

    public async Task<ResponseDTO<bool>> EnsureRolesAsync(string rol)
    {
        try
        {
            if (string.IsNullOrEmpty(rol))
            {
                return ResponseDTO<bool>.Failure("El nombre del rol no puede estar vacío");
            }

            var nameRol = rol.Trim();
            var alreadyExists = await _roleManager.RoleExistsAsync(nameRol);
            if (!alreadyExists)
            {
                var role = new AppRole(nameRol);
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    _logger.LogError(
                        "Error al crear rol {Role}: {Errors}",
                        nameRol,
                        string.Join(", ", result.Errors.Select(e => e.Description))
                    );
                    return ResponseDTO<bool>.Failure($"Error al crear el rol {nameRol}");
                }
                _logger.LogInformation("Rol creado: {Role}", nameRol);
            }
            return ResponseDTO<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asegurar roles");
            return ResponseDTO<bool>.Failure($"Error interno al crear rol: {ex.Message}");
        }
    }

    private async Task<bool> CreateUserIfNotExistsAsync(AppUser user, string role)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email!);
            if (existingUser != null)
            {
                _logger.LogInformation("Usuario {Email} ya existe", user.Email);
                var appUser = (AppUser)existingUser;

                await _userManager.UpdateAsync(appUser);
                return true;
            }

            var result = await _userManager.CreateAsync(user, DefaultPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                _logger.LogInformation(
                    "Usuario {Email} creado exitosamente con rol {Role}",
                    user.Email,
                    role
                );
                return true;
            }

            _logger.LogWarning(
                "Error al crear usuario {Email}: {Errors}",
                user.Email,
                string.Join(", ", result.Errors.Select(e => e.Description))
            );
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario {Email}", user.Email);
            return false;
        }
    }
}
