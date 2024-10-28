using System.ComponentModel.DataAnnotations;
using System.Reflection;
using TecnoCredito.Middlewares;

namespace TecnoCredito.Extensions;

public static class EnumExtensions
{
  public static string GetEnumAttributeValue<TAttribute>(this Enum enumValue, Func<TAttribute, string?> valueSelector) where TAttribute : Attribute
  {
    var enumType = enumValue.GetType();
    var member = enumType.GetMember(enumValue.ToString()).FirstOrDefault();

    if (member is not null)
    {
      var attribute = member.GetCustomAttribute<TAttribute>();
      if (attribute is not null)
      {
        var value = valueSelector(attribute);
        return !string.IsNullOrEmpty(value) ? value : enumValue.ToString();
      }
    }

    return enumValue.ToString();
  }

  public static string GetDisplayName(this Enum enumValue)
  {
    return enumValue.GetEnumAttributeValue<DisplayAttribute>(attr => attr.Name);
  }

  public static string GetDescription(this Enum enumValue)
  {
    return enumValue.GetEnumAttributeValue<DisplayAttribute>(attr => attr.Description);
  }

  public static string? GetSeverity(this Enum enumValue)
  {
    return enumValue.GetEnumAttributeValue<SeverityAttribute>(attr => attr.Value);
  }
}
