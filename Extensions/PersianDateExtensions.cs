using System.Globalization;
using Fusion.Enums;

namespace Fusion.Extensions;

/// <summary>
/// The class provides a collection of functions for working with PersianDate
/// </summary>
public static class PersianDateExtensions
{
    //Initialize values
    private const int Second = 1;
    private const int Minute = 60 * Second;
    private const int Hour = 60 * Minute;
    private const int Day = 24 * Hour;
    private const int Month = 30 * Day;

    /// <summary>
    /// Get persian name of month
    /// </summary>
    /// <param name="month"></param>
    /// <returns></returns>
    public static string ToPersianMonthName(this int month)
    {
        return month switch
        {
            1 => "فروردین",
            2 => "اردیبهشت",
            3 => "خرداد",
            4 => "تیر",
            5 => "مرداد",
            6 => "شهریور",
            7 => "مهر",
            8 => "آبان",
            9 => "آذر",
            10 => "دی",
            11 => "بهمن",
            12 => "اسفند",
            _ => throw new ArgumentOutOfRangeException(nameof(month))
        };
    }

    /// <summary>
    /// Get persian name of day of week
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static string ToPersianDayOfWeekName(this DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Saturday => "شنبه",
            DayOfWeek.Sunday => "یکشنبه",
            DayOfWeek.Monday => "دوشنبه",
            DayOfWeek.Tuesday => "سه شنبه",
            DayOfWeek.Wednesday => "چهارشنبه",
            DayOfWeek.Thursday => "پنجشنبه",
            DayOfWeek.Friday => "جمعه",
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek))
        };
    }

    /// <summary>
    /// Convert date time to shamsi short date as enum pattern
    /// </summary>
    /// <param name="date"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static string ToPersianDate(this DateTime date, PersianDatePattern pattern)
    {
        var pc = new PersianCalendar();
        return pattern switch
        {
            PersianDatePattern.Standard
                => $"{pc.GetYear(date):0000}/{pc.GetMonth(date):00}/{pc.GetDayOfMonth(date):00}",

            PersianDatePattern.Pattern1
                => $"{pc.GetHour(date)}:{pc.GetMinute(date)} | {pc.GetDayOfMonth(date)}-{pc.GetMonth(date).ToPersianMonthName()}",

            PersianDatePattern.Pattern2
                => $"{pc.GetDayOfMonth(date)}-{pc.GetMonth(date).ToPersianMonthName()} {pc.GetHour(date)}:{pc.GetMinute(date)}",

            PersianDatePattern.Pattern3
                => $"{pc.GetYear(date)} {pc.GetMonth(date).ToPersianMonthName()} {pc.GetDayOfMonth(date)} {pc.GetHour(date)}:{pc.GetMinute(date)}",

            PersianDatePattern.Pattern4
                => $"{pc.GetYear(date)}/{pc.GetMonth(date)}/{pc.GetDayOfMonth(date)} {date.Hour}:{date.Minute}",

            PersianDatePattern.Pattern5
                => $"{pc.GetDayOfWeek(date).ToPersianDayOfWeekName()} {pc.GetDayOfMonth(date)} {pc.GetMonth(date).ToPersianMonthName()} {pc.GetYear(date)}",

            PersianDatePattern.Pattern6
                => $"{pc.GetDayOfMonth(date)} {pc.GetMonth(date).ToPersianMonthName()} {pc.GetYear(date)}",

            PersianDatePattern.Pattern7
                => $"{pc.GetDayOfWeek(date).ToPersianDayOfWeekName()} {pc.GetMonth(date)}/{pc.GetDayOfMonth(date)}",

            PersianDatePattern.Pattern8
                => $"{pc.GetDayOfWeek(date).ToPersianDayOfWeekName()} {pc.GetYear(date)}/{pc.GetMonth(date)}/{pc.GetDayOfMonth(date)}",

            PersianDatePattern.Pattern9
                => $"{pc.GetYear(date)}-{pc.GetMonth(date)}-{pc.GetDayOfMonth(date)} {pc.GetHour(date)}:{pc.GetMinute(date)}:{pc.GetSecond(date)}",

            _ => throw new ArgumentOutOfRangeException(nameof(pattern), pattern, null)
        };
    }

    /// <summary>
    /// Convert date time to persian date and send empty string on null datetime
    /// </summary>
    /// <param name="date"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static string ToPersianDate(this DateTime? date, PersianDatePattern pattern)
    {
        return date.HasValue ? date.Value.ToPersianDate(pattern) : string.Empty;
    }
}