namespace TecnoCredito.Middlewares;

[AttributeUsage(AttributeTargets.Field)]
public class SeverityAttribute : Attribute
{
  public string Value { get; }

  public SeverityAttribute(string value)
  {
    Value = value;
  }
}
