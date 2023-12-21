using CloudinaryDotNet.Actions;

namespace API;

public interface IPhotoService
{

    Task<ImageUploadResult> AddImageAsync(IFormFile file);

    Task<DeletionResult> DeleteImageAsync(string publicId);

}

