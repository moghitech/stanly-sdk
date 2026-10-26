using Fusion.Constants;

namespace Fusion.Helpers.Storage;

/// <summary>
/// 
/// </summary>
public static class ObjectStorageHelpers
{
    /// <summary>
    /// Just return content type of the object according to its file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetContentType(string fileExtension)
    {
        return fileExtension.ToLower() switch
        {
            "jpg" or ".jpeg" => ObjectTypeConstants.Jpeg,
            "png" => ObjectTypeConstants.Png,
            "gif" => ObjectTypeConstants.Gif,
            "bmp" => ObjectTypeConstants.Bmp,
            "tiff" => ObjectTypeConstants.Tiff,
            "mp4" => ObjectTypeConstants.Mp4,
            "webm" => ObjectTypeConstants.WebM,
            "pdf" => ObjectTypeConstants.Pdf,
            "mp3" => ObjectTypeConstants.Mp3,
            "ogg" => ObjectTypeConstants.Ogg,
            "wav" => ObjectTypeConstants.Wav,
            _ => throw new ArgumentException("Unsupported file type")
        };
    }
}