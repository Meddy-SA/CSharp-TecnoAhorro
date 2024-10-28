namespace TecnoCredito.Models.Authentication.DTOs;

public class PersonalDataDTO
{
  public string FirstName { get; set; } = null!;
  public string MiddleName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public string SecondSurName { get; set; } = null!;
  public string? Avatar { get; set; }
  public bool LoginWith2FA { get; set; } = false;

  public void LoadData(string f, string l, string m = "", string s = "")
  {
    FirstName = f;
    LastName = l;
    MiddleName = m;
    SecondSurName = s;
  }
}
