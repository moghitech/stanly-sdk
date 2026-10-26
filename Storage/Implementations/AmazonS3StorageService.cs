using Amazon.S3;
using Amazon.S3.Model;
using Fusion.Helpers.Storage;
using Fusion.Storage.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Fusion.Storage.Implementations;

/// <summary>
/// The amazon s3 implementation for manage object storage.
/// </summary>
internal sealed class AmazonS3StorageService : IObjectStorageService
{
    private readonly ObjectStorageSettings _objectStorage;
    private readonly IAmazonS3 _amazonS3;

    /// <summary>
    /// This specifies which signing algorithm should be used when constructing the signature for the request.
    /// </summary>
    private const string SignatureVersion = "4";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="minioOptionsSnapshot"></param>
    public AmazonS3StorageService(IOptionsSnapshot<ObjectStorageSettings> minioOptionsSnapshot)
    {
        _objectStorage = minioOptionsSnapshot.Value;

        var config = new AmazonS3Config
        {
            ServiceURL = _objectStorage.EndPoint,
            ForcePathStyle = true,
            SignatureVersion = SignatureVersion
        };

        var credentials = new Amazon.Runtime.BasicAWSCredentials(_objectStorage.AccessKey, _objectStorage.SecretKey);

        _amazonS3 = new AmazonS3Client(credentials, config);
    }

    /// <inheritdoc cref="IObjectStorageService.UploadByFormFileAsync"/>
    public async Task UploadByFormFileAsync(string address, IFormFile file, CancellationToken cancellationToken)
    {
        await using var fs = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = _objectStorage.BucketName,
            Key = address,
            InputStream = fs,
            ContentType = file.ContentType,
            CannedACL = S3CannedACL.PublicRead
        };

        await _amazonS3.PutObjectAsync(request, cancellationToken);
    }

    /// <inheritdoc cref="IObjectStorageService.UploadByStreamAsync"/>
    public async Task UploadByStreamAsync(string address, Stream stream, CancellationToken cancellationToken)
    {
        var request = new PutObjectRequest
        {
            BucketName = _objectStorage.BucketName,
            Key = address,
            InputStream = stream,
            CannedACL = S3CannedACL.PublicRead
        };

        await _amazonS3.PutObjectAsync(request, cancellationToken);
    }

    /// <inheritdoc cref="IObjectStorageService.DownloadByAddressAsync"/>
    public async Task<Stream> DownloadByAddressAsync(string address, CancellationToken cancellationToken)
    {
        var response = await _amazonS3.GetObjectAsync(_objectStorage.BucketName, address, cancellationToken);
        var memoryStream = new MemoryStream();

        await using var responseStream = response.ResponseStream;
        await responseStream.CopyToAsync(memoryStream, cancellationToken);

        memoryStream.Position = 0;

        return memoryStream;
    }

    /// <inheritdoc cref="IObjectStorageService.GeneratePreSignedPutObjectUrlAsync"/>
    public async Task<string> GeneratePreSignedPutObjectUrlAsync(string address, string objectName, DateTime expires,
        CancellationToken cancellationToken)
    {
        var contentType = ObjectStorageHelpers.GetContentType(objectName.Split('.')[1]);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _objectStorage.BucketName,
            Key = address,
            Verb = HttpVerb.PUT,
            Expires = expires,
            ContentType = contentType,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
            Metadata =
            {
                ["file-name"] = address.Split('/')[1]
            }
        };

        return await _amazonS3.GetPreSignedURLAsync(request);
    }

    /// <inheritdoc cref="IObjectStorageService.GeneratePreSignedGetObjectUrlAsync"/>
    public async Task<string> GeneratePreSignedGetObjectUrlAsync(string address, DateTime expires,
        CancellationToken cancellationToken)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _objectStorage.BucketName,
            Key = address,
            Verb = HttpVerb.GET,
            Expires = expires
        };

        var preSignedGetUrl = await _amazonS3.GetPreSignedURLAsync(request);

        return preSignedGetUrl;
    }

    /// <inheritdoc cref="IObjectStorageService.DeleteAsync"/>
    public async Task DeleteAsync(string address, CancellationToken cancellationToken)
        => await _amazonS3.DeleteObjectAsync(_objectStorage.BucketName, address, cancellationToken);
}