using FILE_UPLOAD_API.DTO;
using FILE_UPLOAD_API.Models;

namespace FILE_UPLOAD_API.Repositories
{
    public interface IStorageRepository
    {
        Task<ResponseManager<string>> GetFilePathAsync(ViewGetSavedFile file);
        Task<ResponseManager<ReturnFilePath>> GetFilePathListAsync(List<ViewGetSavedFile> file);
        Task<ResponseManager<MemoryStream>> DownloadFileAsync(ViewGetSavedFile file);
        Task<ResponseManager<string>> GetFileBase64Async(ViewGetSavedFile file);
        Task<ResponseManager<SavedFileDTO>> UploadFileAsync(UploadFileDTO model);
        Task<ResponseManager<SavedFileDTO>> UploadFileFromBase64Async(UploadFileFromBase64DTO model);
        Task<ResponseManager> DeleteFileAsync(ViewGetSavedFile file, int userId);

    }
}
