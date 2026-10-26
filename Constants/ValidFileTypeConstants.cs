namespace Fusion.Constants;

internal static class ValidFileTypeConstants
{
    /// <summary>
    /// validFile acceptable file extensions
    /// </summary>
    internal static List<string> ChatAcceptableFileTypes =>
        new() { ".pdf", ".doc", ".docx", ".zip", ".rar" };

    /// <summary>
    /// Sound acceptable file extensions
    /// </summary>
    internal static List<string> ValidSoundFileTypes =>
        new() { ".mp3", ".m4a", ".wave", ".mp4", ".avi", ".aac", ".ogg", ".acc" };

    /// <summary>
    /// Image acceptable file extensions
    /// </summary>
    internal static List<string> ValidImageFileTypes => new() { ".jpg", ".jpeg", ".gif", ".png" };

    /// <summary>
    /// Pdf acceptable file extensions
    /// </summary>
    internal static List<string> ValidPdfFileTypes => new() { ".pdf" };

    /// <summary>
    /// Excel acceptable file extensions
    /// </summary>
    internal static List<string> ValidExcelFileTypes = new() { ".csv", ".xls", ".xlsx" };
}
