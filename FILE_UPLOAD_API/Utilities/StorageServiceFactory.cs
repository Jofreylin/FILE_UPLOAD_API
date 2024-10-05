using FILE_UPLOAD_API.DTO;
using FILE_UPLOAD_API.Repositories;
using FILE_UPLOAD_API.Services;

namespace FILE_UPLOAD_API.Utilities
{
    public class StorageServiceFactory
    {
        private readonly IServiceProvider serviceProvider;

        public StorageServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IStorageRepository CreateService(int? storageTypeId)
        {
            switch (storageTypeId)
            {
                case (int)Utility.StorageTypes.Local:
                    return serviceProvider.GetRequiredService<LocalStorageService>();

                case (int)Utility.StorageTypes.S3:
                    return serviceProvider.GetRequiredService<S3StorageService>();

                case (int)Utility.StorageTypes.FTP:
                    return serviceProvider.GetRequiredService<FTPStorageService>();

                default:
                    return serviceProvider.GetRequiredService<LocalStorageService>();
            }
        }
    }
}
