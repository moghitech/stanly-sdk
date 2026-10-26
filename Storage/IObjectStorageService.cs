using Microsoft.AspNetCore.Http;

namespace Fusion.Storage;

/// <summary>
/// The main abstraction of object storage to manage files and objects.
/// </summary>
public interface IObjectStorageService
{
    /// <summary>
    /// Upload an object using default form file of c#.
    /// </summary>
    /// <param name="address">The address of object to save</param>
    /// <param name="file">The file ot upload</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UploadByFormFileAsync(string address, IFormFile file, CancellationToken cancellationToken);

    /// <summary>
    /// Uploads an object using stream.
    /// </summary>
    /// <param name="address">The address of object to save</param>
    /// <param name="stream">The stream to save file</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UploadByStreamAsync(string address, Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Downloads an object using its address
    /// </summary>
    /// <param name="address">The address of object</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream> DownloadByAddressAsync(string address, CancellationToken cancellationToken);

    /// <summary>
    /// Generates pre-signed url for putting object.
    /// </summary>
    /// <param name="address">The address of object</param>
    /// <param name="objectName">The name of object</param>
    /// <param name="expires">The expiry time of url</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GeneratePreSignedPutObjectUrlAsync(string address, string objectName, DateTime expires,
        CancellationToken cancellationToken);

    /// <summary>
    /// Generates pre-signed url for putting object.
    /// </summary>
    /// <param name="address">The address of object</param>
    /// <param name="expires">Date and Time to expire url</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GeneratePreSignedGetObjectUrlAsync(string address, DateTime expires,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an object using its address.
    /// </summary>
    /// <param name="address">The address of object</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(string address, CancellationToken cancellationToken);
}