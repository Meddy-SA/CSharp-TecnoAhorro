namespace TecnoCredito.Models.Authentication.DTOs;

public class PersonNameModel
{
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public string? SecondSurname { get; set; }
}
