namespace Fusion.Enums;

/// <summary>
/// Represents the days of the week.
/// Allows enumeration of days typically used for scheduling or identification of specific weekdays.
/// </summary>
public enum DaysOfWeek : byte
{
    /// <summary>
    /// Represents Saturday.
    /// </summary>
    Saturday = 1,

    /// <summary>
    /// Represents Sunday.
    /// </summary>
    Sunday = 2,

    /// <summary>
    /// Represents Monday.
    /// </summary>
    Monday = 4,

    /// <summary>
    /// Represents Tuesday.
    /// </summary>
    Tuesday = 8,

    /// <summary>
    /// Represents Wednesday.
    /// </summary>
    Wednesday = 16,

    /// <summary>
    /// Represents Thursday.
    /// </summary>
    Thursday = 32,

    /// <summary>
    /// Represents Friday.
    /// </summary>
    Friday = 64
}