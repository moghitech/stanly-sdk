using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Fusion.Helpers.DTOs;

namespace Fusion.Helpers;

/// <summary>
/// The helper class for working on Enum types
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// Get enum value from display name
    /// </summary>
    /// <param name="displayName">DisplayName of the enum</param>
    /// <typeparam name="TEnum">Generic type of the enum</typeparam>
    /// <returns>Enum value</returns>
    public static TEnum GetValueFromName<TEnum>(string displayName)
        where TEnum : struct, Enum
    {
        var type = typeof(TEnum);
        if (!type.IsEnum)
            throw new InvalidOperationException($"The input is not an enum. [Type] = ({type.FullName}), [DisplayName] = ({displayName})");

        foreach (var field in type.GetFields())
        {
            if (
                Attribute.GetCustomAttribute(field, typeof(DisplayAttribute))
                is DisplayAttribute attribute
            )
            {
                if (attribute.Name == displayName)
                {
                    return (TEnum)field.GetValue(null)!;
                }
            }
            else
            {
                if (field.Name == displayName)
                    return (TEnum)field.GetValue(null)!;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(displayName));
    }

    /// <summary>
    /// get value information list
    /// </summary>
    /// <typeparam name="TEnum">pass enum as base type</typeparam>
    /// <returns></returns>
    public static IEnumerable<EnumItemInformationDto> GetValueInformationList<TEnum>()
        where TEnum : struct, Enum
    {
        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            var displayAttribute = enumValue
                .GetType()
                .GetField(enumValue.ToString())
                ?.GetCustomAttribute<DisplayAttribute>();

            var enumInformation = new EnumItemInformationDto(
                Convert.ToByte(enumValue), 
                displayAttribute?.Description,
                displayAttribute?.Name
            );

            yield return enumInformation;
        }
    }

    /// <summary>
    /// get value information list based on GroupNames
    /// </summary>
    /// <typeparam name="TEnum">pass enum as base type</typeparam>
    /// <returns></returns>
    public static IEnumerable<EnumItemInformationDto> GetValueInformationList<TEnum>(params string[] groupNames)
        where TEnum : struct, Enum
    {
        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            var displayAttribute = enumValue
                .GetType()
                .GetField(enumValue.ToString())
                ?.GetCustomAttribute<DisplayAttribute>();
            if (!groupNames.Contains(displayAttribute?.GroupName))
                continue;

            var enumInformation = new EnumItemInformationDto(
                Convert.ToByte(enumValue), 
                displayAttribute?.Description,
                displayAttribute?.Name
            );

            yield return enumInformation;
        }
    }
}