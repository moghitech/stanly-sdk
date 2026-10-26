using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Fusion.Messaging.RabbitMq.Helpers;

/// <summary>
///     Utility methods for converting arbitrary strings into RabbitMQ‑friendly queue names that follow the
///     "lowercase words separated by dots or hyphens" convention.
/// </summary>
internal static class EventNameHelpers
{
    /// <summary>
    ///     Converts <paramref name="input"/> into a lowercase, separator‑delimited queue name.
    ///     Any character that is not a letter or digit is replaced by the <paramref name="separator"/>. Multiple
    ///     consecutive separators are collapsed, and the result is trimmed to 255 characters — the maximum length
    ///     accepted by RabbitMQ.
    /// </summary>
    /// <param name="input">Source text to convert.</param>
    /// <param name="separator">
    ///     The separator to use between words. Pass '.' for dot‑style names (default) or '-' for hyphen style.
    /// </param>
    /// <returns>A sanitized queue name suitable for RabbitMQ.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="input"/> is null or empty.</exception>
    public static string NormalizeName(string input, char separator = '.')
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be null or empty", nameof(input));

        if (separator != '.' && separator != '-')
            throw new ArgumentException("Separator must be either '.' or '-'", nameof(separator));

        // 1. Normalise case and trim
        var normalized = input.Trim().ToLowerInvariant();

        // 2. Remove diacritics (e.g., é → e)
        normalized = RemoveDiacritics(normalized);

        // 3. Replace invalid chars with the chosen separator
        var sb = new StringBuilder(capacity: normalized.Length);
        foreach (var c in normalized)
        {
            // Space, underscore or any other disallowed char becomes the separator
            sb.Append(IsAllowedChar(c) ? c : separator);
        }

        var candidate = sb.ToString();

        // 4. Collapse duplicate separators and trim leading/trailing ones
        var pattern = Regex.Escape(separator.ToString()) + "+"; // e.g. "\\.+" or "-+"
        candidate = Regex.Replace(candidate, pattern, separator.ToString());
        candidate = candidate.Trim(separator);

        // 5. Enforce RabbitMQ 255‑byte limit
        if (candidate.Length > 255)
            candidate = candidate[..255];

        return candidate;
    }

    private static bool IsAllowedChar(char c) => c is >= 'a' and <= 'z' || char.IsDigit(c);

    private static string RemoveDiacritics(string text)
    {
        var formD = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(formD.Length);

        foreach (var ch in formD.Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark))
        {
            sb.Append(ch);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}