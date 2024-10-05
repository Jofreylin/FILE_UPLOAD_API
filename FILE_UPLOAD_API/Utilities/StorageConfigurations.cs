namespace FILE_UPLOAD_API.Utilities
{
    public class S3Credentials
    {
        public string Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Bucket { get; set; }
        public string ServiceURL { get; set; }
    }

    public class FTPCredentials
    {
        public string Host { get; set; }
        public string UrlHost { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        
    }
}
