namespace Fusion.Enums;

/// <summary>
/// The enum type holds the types of PersianDate patterns
/// </summary>
public enum PersianDatePattern
{
    /// <summary>
    /// [Year]/[Month]/[Day]
    /// </summary>
    Standard,

    /// <summary>
    /// [Hour]:[Minute] | [Day]-[MonthName]
    /// </summary>
    Pattern1,

    /// <summary>
    /// [Day]-[MonthName] [Hour]:[Minute]
    /// </summary>
    Pattern2,

    /// <summary>
    /// [Year] [MonthName] [Day] [Hour]:[Minute]
    /// </summary>
    Pattern3,

    /// <summary>
    /// [Year]/[Month]/[Day] [Hour]:[Minute]
    /// </summary>
    Pattern4,

    /// <summary>
    /// [WeekDay] [Day] [MonthName] [Year]
    /// </summary>
    Pattern5,

    /// <summary>
    /// [Day] [MonthName] [Year]
    /// </summary>
    Pattern6,

    /// <summary>
    /// [WeekDay] [Day]/[Month]
    /// </summary>
    Pattern7,

    /// <summary>
    /// [WeekDay] [Day]/[Month]/[Year]
    /// </summary>
    Pattern8,

    /// <summary>
    /// [Year]-[Month]-[Day] [Hour]:[Minute]:[Sec]
    /// </summary>
    Pattern9
}
