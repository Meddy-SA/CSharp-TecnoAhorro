using System.ComponentModel.DataAnnotations;
using System.Reflection;
using TecnoCredito.Middlewares;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.Enums;

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

  public static string GetDescription(this Enum enumValue)
  {
    return enumValue.GetEnumAttributeValue<DisplayAttribute>(attr => attr.Description);
  }

  public static string? GetSeverity(this Enum enumValue)
  {
    return enumValue.GetEnumAttributeValue<SeverityAttribute>(attr => attr.Value);
  }

  public static string GetDisplayName(this Enum value)
  {
    var field = value.GetType().GetField(value.ToString());
    var attribute = field?.GetCustomAttribute<DisplayAttribute>();

    return attribute?.Name ?? value.ToString();
  }

  public static List<EnumDTO> ToEnumValueList<TEnum>() where TEnum : Enum
  {
    return Enum.GetValues(typeof(TEnum))
        .Cast<TEnum>()
        .Select(e => new EnumDTO
        {
          Id = Convert.ToInt32(e),
          Name = e.GetDisplayName()
        })
        .ToList();
  }

  public static List<RolesEnum> GetListRoleForName(IList<string> roles)
  {
    var enumType = typeof(RolesEnum);
    var roleList = new List<RolesEnum>();

    foreach (var role in roles)
    {
      if (Enum.IsDefined(enumType, role))
      {
        roleList.Add((RolesEnum)Enum.Parse(enumType, role));
      }
    }

    if (roleList.Count == 0)
    {
      roleList.Add(RolesEnum.Visitante);
    }

    return roleList;
  }

  public static TEnum? GetEnumFromValue<TEnum>(int value) where TEnum : struct, Enum
  {
    if (Enum.IsDefined(typeof(TEnum), value))
    {
      return (TEnum)Enum.ToObject(typeof(TEnum), value);
    }
    return null;
  }

  public static string GetDisplayNameFromValue<TEnum>(int value) where TEnum : struct, Enum
  {
    var enumValue = GetEnumFromValue<TEnum>(value);
    return enumValue?.GetDisplayName() ?? enumValue.ToString() ?? string.Empty;
  }
}
