using Microsoft.AspNetCore.StaticFiles;

namespace FILE_UPLOAD_API.Utilities
{
    public static class Utility
    {
        public enum DbOperations
        {
            Insert = 1,
            Update = 2,
            Delete = 3
        }

        public enum StorageTypes
        {
            Local = 1,
            S3 = 2,
            Azure = 3,
            FTP = 4
        }

        public static string CreatefilePath(IWebHostEnvironment Environment, string Folder, string FileName)
        {
            var directoryPath = Path.Combine(Environment.WebRootPath, Folder);
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            return Path.Combine(directoryPath, FileName);
        }

        public static string? GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out string? contentType))
            {
                contentType = null;
            }

            return contentType;
        }

        public static string? GetPath(IWebHostEnvironment Environment, Guid GuidName)
        {
            string searchPattern = GuidName + Constants.AllExtension;
            string[] files = Directory.GetFiles(Path.Combine(Environment.WebRootPath, Constants.FolderSaveFile), searchPattern);
            var path = files.FirstOrDefault();
            if (!File.Exists(path))
            {
                return null;
            }
            return path;
        }

        public static string? GetPathByFileName(IWebHostEnvironment Environment, string FileName)
        {
            string searchPattern = FileName;
            string[] files = Directory.GetFiles(Path.Combine(Environment.WebRootPath, Constants.FolderSaveFile), searchPattern);
            var path = files.FirstOrDefault();
            if (!File.Exists(path))
            {
                return null;
            }
            return path;
        }

        public static string[] GetFiles(IWebHostEnvironment Environment)
        {
            return Directory.GetFiles(Path.Combine(Environment.WebRootPath, Constants.FolderSaveFile));
        }

        public static string GetUrlToViewFile(string apiBaseUrl, string fileName)
        {
            return $"{apiBaseUrl}/api/Document/ViewDocument?FileName={fileName}";
        }
    }

    public static class Constants
    {
        public const string FolderSaveFile = "UPLOADED-FILES";
        public const string AllExtension = ".*";
    }

}
