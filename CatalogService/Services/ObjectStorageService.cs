namespace CatalogService.Services;
using CatalogService.Configuration;
using Microsoft.Extensions.Options;

public class ObjectStorageService
{
    private readonly Amazon.S3.IAmazonS3 _s3Client;
    private readonly ObjectStorageOptions _options;

    public ObjectStorageService(Amazon.S3.IAmazonS3 s3Client, IOptions<ObjectStorageOptions> options)
    {
        _s3Client = s3Client;
        _options = options.Value;
    }

    public async Task UploadAsync(string key, Stream fileStream, string contentType)
    {
        var putRequest = new Amazon.S3.Model.PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType
        };

        await _s3Client.PutObjectAsync(putRequest);
    }

    public async Task<Amazon.S3.Model.GetObjectResponse> DownloadAsync(string key)
    {
        var getRequest = new Amazon.S3.Model.GetObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key
        };

        var response = await _s3Client.GetObjectAsync(getRequest);
        return response;
    }

    public async Task DeleteAsync(string key)
    {
        var deleteRequest = new Amazon.S3.Model.DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(deleteRequest);
    }

    public async Task<string> GetPresignedUrl(string key, TimeSpan expiration)
    {
        var request = new Amazon.S3.Model.GetPreSignedUrlRequest{
            BucketName = _options.BucketName,
            Key = key,
            Verb = Amazon.S3.HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expiration)
        };
        return _s3Client.GetPreSignedURL(request);
    }
}
