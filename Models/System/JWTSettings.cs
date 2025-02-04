namespace TecnoCredito.Models.System;

public class JwtSettings
{
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string LifeType { get; set; } = null!;
    public int TokenLifeTime { get; set; }
}
