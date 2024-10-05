namespace FILE_UPLOAD_API.DTO
{

    public class UploadFileDTO
    {
        public IFormFile? File { get; set; }
        public int CategoryId { get; set; }
        public int? UserId { get; set; }
        public int CompanyId { get; set; }
        public int StorageTypeId { get; set; }
    }

    public class UploadFileFromBase64DTO
    {
        public int CategoryId { get; set; }
        public int? UserId { get; set; }
        public int CompanyId { get; set; }
        public int StorageTypeId { get; set; }
        public string? Base64 { get; set; }
        public string? Extension { get; set; }
    }

    public class ReturnFilePath
    {
        public string? Path { get; set; }
        public Guid FileName { get; set; }
        public bool Found { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
