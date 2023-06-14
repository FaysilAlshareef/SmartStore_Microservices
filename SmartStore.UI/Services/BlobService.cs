using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SmartStore.UI.Services.Interfaces;

namespace SmartStore.UI.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }
    public async Task<bool> DeleteBlob(string blobName, string containerName)
    {
        var contaunerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient = contaunerClient.GetBlobClient(blobName);
        return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<string> UploadBlob(string blobName, IFormFile file, string containerName)
    {
        var contaunerClient = _blobServiceClient.GetBlobContainerClient(containerName);


        //checking if file is exists
        //if file exists it will be replaced
        //if it doesn't exist it will create a temp space until it is uploaded

        var blobClient = contaunerClient.GetBlobClient(Guid.NewGuid() + blobName);
        var httpHeaders = new BlobHttpHeaders()
        {
            ContentType = file.ContentType
        };

        await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);
        return blobClient.Uri.AbsoluteUri;
    }
}
