using TecnoCredito.Models.Authentication.DTOs;

namespace TecnoCredito.Helpers;

public static class NameParserHelper
{
    public static PersonNameModel ParseFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException(
                "El nombre completo no puede estar vacío.",
                nameof(fullName)
            );

        // Limpiamos espacios múltiples y dividimos la cadena
        var nameParts = fullName
            .Trim()
            .Replace("  ", " ")
            .Split(' ')
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var result = new PersonNameModel();

        // Si solo viene un nombre
        if (nameParts.Count == 1)
        {
            result.FirstName = nameParts[0];
            result.LastName = "Sin Apellido";
            return result;
        }

        // Lógica para asignar las partes del nombre
        switch (nameParts.Count)
        {
            case 2: // Nombre y apellido
                result.FirstName = nameParts[0];
                result.LastName = nameParts[1];
                break;

            case 3: // Puede ser: Nombre + 2 apellidos O 2 nombres + apellido
                // Heurística: Si el segundo elemento empieza con mayúscula, asumimos que es parte del nombre
                if (char.IsUpper(nameParts[1][0]) && nameParts[1].Length > 2)
                {
                    result.FirstName = nameParts[0];
                    result.MiddleName = nameParts[1];
                    result.LastName = nameParts[2];
                }
                else
                {
                    result.FirstName = nameParts[0];
                    result.LastName = nameParts[1];
                    result.SecondSurname = nameParts[2];
                }
                break;

            default: // 4 o más partes
                result.FirstName = nameParts[0];
                result.MiddleName = nameParts[1];
                result.LastName = nameParts[2];
                result.SecondSurname = string.Join(" ", nameParts.Skip(3));
                break;
        }

        return result;
    }

    // Método de extensión opcional para convertir el modelo a string
    public static string ToFullName(this PersonNameModel name)
    {
        var parts = new List<string> { name.FirstName };

        if (!string.IsNullOrWhiteSpace(name.MiddleName))
            parts.Add(name.MiddleName);

        parts.Add(name.LastName);

        if (!string.IsNullOrWhiteSpace(name.SecondSurname))
            parts.Add(name.SecondSurname);

        return string.Join(" ", parts);
    }
}
