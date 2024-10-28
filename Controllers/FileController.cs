using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TecnoCredito.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class FileController(IWebHostEnvironment environment, IConfiguration configuration) : ControllerBase
{
  private readonly IWebHostEnvironment environment = environment;
  private readonly IConfiguration configuration = configuration;

  [HttpPost("Upload")]
  public async Task<IActionResult> Upload(List<IFormFile> files)
  {
    try
    {
      var maxFileSize = configuration.GetSection("FileUpload:MaxFileSizeBytes").Get<int>();
      var allowedExtensions = configuration.GetSection("FileUpload:AllowedExtensions").Get<string[]>()!;

      var uploadResults = new List<object>();

      foreach (var file in files)
      {
        if (file.Length > 0)
        {

          var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

          // Validar tipo de archivo
          if (!allowedExtensions.Contains(extension))
          {
            return BadRequest($"El archivo {file.FileName} no es una imagen válida.");
          }

          // Generar un nombre de archivo unico
          var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

          // Definir la ruta donde se guardara el archivo
          var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
          var filePath = Path.Combine(uploadsFolder, fileName);

          // Asegurar de que la carpeta exista
          Directory.CreateDirectory(uploadsFolder);

          // Guardar el archivo
          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await file.CopyToAsync(stream);
          }

          // Añadir el resultado a la lista.
          uploadResults.Add(new
          {
            FileName = fileName,
            Size = file.Length,
            Url = $"uploads/{fileName}"
          });
        }

      }

      return Ok(new { Message = "Archivos subidos correctamente", Files = uploadResults });
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"Error al subir el archivo: {ex.Message}");
    }
  }

}
