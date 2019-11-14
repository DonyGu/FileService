namespace FileService.Application.Interfaces
{
    public interface IFileRateLimitingService
    {
        // call before handle upload file request
        // throw RateLimitingException when number of requests exceeds limit
        void CheckUpload(string ip);

        // call before handle get file request
        // throw RateLimitingException when requests exceeds limit
        void CheckGet(string ip, string fileKey);
    }
}