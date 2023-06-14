namespace SmartStore.UI.Services.Interfaces;

public interface IBlobService
{
    Task<string> UploadBlob(string blobName, IFormFile file, string containerName);
    Task<bool> DeleteBlob(string blobName, string containerName);

}
