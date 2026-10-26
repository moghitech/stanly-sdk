namespace Fusion.Helpers.DTOs;

/// <summary>
/// Enum value information dto
/// </summary>
/// <param name="Value">the number value of enum</param>
/// <param name="DisplayName">Name property of Display attribute</param>
/// <param name="DisplayDescription">Description property of Display attribute</param>
public record EnumItemInformationDto(
    int Value, 
    string? DisplayName,
    string? DisplayDescription
);