namespace FastLane.Service
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile file, string folderName);
        void DeleteImageAsync(string imagePath);
    }
}
