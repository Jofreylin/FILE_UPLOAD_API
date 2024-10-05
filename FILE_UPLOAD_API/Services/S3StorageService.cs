using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using FILE_UPLOAD_API.DTO;
using FILE_UPLOAD_API.Models;
using FILE_UPLOAD_API.Repositories;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;
using System.ComponentModel.Design;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FILE_UPLOAD_API.Utilities;
using Microsoft.Extensions.Options;
using Amazon.Runtime;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System;
using FILE_UPLOAD_API.Context;
using System.Buffers.Text;
using System.IO;

namespace FILE_UPLOAD_API.Services
{
    public class S3StorageService : IStorageRepository
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _awsS3Client;
        private readonly ILogRepository _logRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly S3Credentials _s3Credentials;
        private readonly DocApiContext _context;

        public S3StorageService(ILogRepository logRepository, IDocumentRepository documentRepository, IOptions<S3Credentials> s3Credentials, DocApiContext context)
        {
            _logRepository = logRepository;
            _documentRepository = documentRepository;
            _context = context;

            _s3Credentials = s3Credentials?.Value;

            var awsConfig = new AmazonS3Config
            {
                ServiceURL = _s3Credentials.ServiceURL
                //RegionEndpoint = RegionEndpoint.GetBySystemName(_s3Credentials.Region)
            };

            var awsCredentials = new BasicAWSCredentials(_s3Credentials.AccessKey, _s3Credentials.SecretKey);

            string bucketName = _s3Credentials.Bucket;

            _bucketName = bucketName;
            _awsS3Client = new AmazonS3Client(awsCredentials, awsConfig);

        }

        public async Task<ResponseManager<string>> GetFileBase64Async(ViewGetSavedFile file)
        {
            var result = new ResponseManager<string>();

            try
            {
                var responseMemory = await DownloadFileAsync(file);
                if (!responseMemory.Succeded)
                {
                    result.Errors.AddRange(responseMemory.Errors);
                    return result;
                }

                byte[] bytes = responseMemory.SingleData.ToArray();

                // Convierte el array de bytes a una cadena en base64
                string base64String = Convert.ToBase64String(bytes);

                result.SingleData = base64String;

            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error consultando la lista de documentos. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(file);
                _logRepository.InsertLog("S3StorageService.GetFileBase64Async", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager<string>> GetFilePathAsync(ViewGetSavedFile file)
        {
            var result = new ResponseManager<string>();

            try
            {
                if (string.IsNullOrEmpty(file?.RouteSaved))
                {
                    result.Errors.Add("El documento solicitado no posee una ruta asignada para poder consultado. Favor de Comunicarse con un administrador");
                    return result;
                }

                var existsFile = await CheckIfFileExistsAsync(file.RouteSaved);

                if (!existsFile)
                {
                    result.Errors.Add("El documento solicitado no pudo ser encontrado en la ruta especificada. Favor de Comunicarse con un administrador");
                    return result;
                }

                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = file.RouteSaved,
                    Expires = DateTime.UtcNow.AddMinutes(90)
                };

                var url = _awsS3Client.GetPreSignedURL(request);

                result.SingleData = url;

            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error consultando la lista de documentos. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(file);
                _logRepository.InsertLog("S3StorageService.GetFilePathAsync", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager<ReturnFilePath>> GetFilePathListAsync(List<ViewGetSavedFile> files)
        {
            var result = new ResponseManager<ReturnFilePath>();

            try
            {
                var list = new List<ReturnFilePath>();

                var tasks = files.Select(file => Task.Run(async () =>
                {
                    var fileToReturn = new ReturnFilePath
                    {
                        FileName = file.FileName
                    };

                    var response = await GetFilePathAsync(file);
                    if (!response.Succeded)
                    {
                        fileToReturn.Found = false;
                        fileToReturn.ErrorMessage = response.Errors[0];
                        list.Add(fileToReturn);
                        return;
                    }

                    fileToReturn.Path = response.SingleData;
                    fileToReturn.Found = true;

                    list.Add(fileToReturn);

                }));

                await Task.WhenAll(tasks);

                result.DataList = list;

            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error consultando la lista de documentos. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(files);
                _logRepository.InsertLog("S3StorageService.GetFilePathListAsync", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager<MemoryStream>> DownloadFileAsync(ViewGetSavedFile file)
        {
            var result = new ResponseManager<MemoryStream>();

            try
            {
                if (string.IsNullOrEmpty(file?.RouteSaved))
                {
                    result.Errors.Add("El documento solicitado no posee una ruta asignada para poder consultado. Favor de Comunicarse con un administrador");
                    return result;
                }

                var existsFile = await CheckIfFileExistsAsync(file.RouteSaved);

                if (!existsFile)
                {
                    result.Errors.Add("El documento solicitado no pudo ser encontrado en la ruta especificada. Favor de Comunicarse con un administrador");
                    return result;
                }

                var request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = file.RouteSaved
                };

                var objectRequested = await _awsS3Client.GetObjectAsync(request);

                using var memory = new MemoryStream();

                await objectRequested.ResponseStream.CopyToAsync(memory);

                result.SingleData = memory;

            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error consultando la lista de documentos. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(file);
                _logRepository.InsertLog("S3StorageService.DownloadFileAsync", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager<SavedFileDTO>> UploadFileAsync(UploadFileDTO model)
        {
            var result = new ResponseManager<SavedFileDTO>();
            try
            {
                var file = model.File;

                if (file is null || file.Length <= 0)
                {
                    result.Errors.Add("No se encontró un archivo para ser subido en esta petición");
                    return result;
                }

                if (model.CompanyId <= 0)
                {
                    result.Errors.Add("La institución enviada en esta petición no es valida");
                    return result;
                }

                Guid fileNameGUID = Guid.NewGuid();

                while (true)
                {
                    var responseFileList = await _documentRepository.GetSavedFilesList(0, fileNameGUID, 0, 0, model.StorageTypeId, model.CompanyId, type: 0);
                    if (!responseFileList.Succeded)
                    {
                        result.Errors.AddRange(responseFileList.Errors);
                        return result;
                    }

                    if (responseFileList.DataList.Any())
                    {
                        fileNameGUID = Guid.NewGuid();
                    }
                    else
                    {
                        break;
                    }
                }

                var responseCategoriesList = await _documentRepository.GetCategoriesList(model.CategoryId, 0, null, model.CompanyId, type: 1);
                if (!responseCategoriesList.Succeded)
                {
                    result.Errors.AddRange(responseCategoriesList.Errors);
                    return result;
                }

                var category = responseCategoriesList.DataList.FirstOrDefault(x => x.CategoryId == model.CategoryId);

                if (category is null)
                {
                    result.Errors.Add("La categoria seleccionada no ha sido encontrada. Favor de comunicarse con un administrador");
                    return result;
                }

                string originalFileName = file.FileName.Trim('"');
                string originalExtension = Path.GetExtension(originalFileName);

                string fileWithPath = $"{model.CompanyId}/{category.Directory}/{fileNameGUID}{originalExtension}";

                var modelToInsert = new SavedFileDTO
                {
                    FileName = fileNameGUID,
                    OriginalFileExtension = originalExtension,
                    OriginalFileName = originalFileName,
                    RouteSaved = fileWithPath,
                    StorageTypeId = model.StorageTypeId,
                    DocumentCategoryId = model.CategoryId,
                    CompanyId = model.CompanyId,
                    UserId = model.UserId
                };


                using var fileStream = file.OpenReadStream();

                var objectRequest = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = fileWithPath,
                    InputStream = fileStream,
                    ContentType = file.ContentType
                };

                await _awsS3Client.PutObjectAsync(objectRequest);

                var responseSaveFileRecord = await _documentRepository.InsertSavedFileData(modelToInsert);
                if (!responseSaveFileRecord.Succeded)
                {
                    result.Errors.AddRange(responseSaveFileRecord.Errors);

                    var sendResponse = SendDeleteRequestAsync(fileWithPath);

                    return result;
                }

                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = modelToInsert.RouteSaved,
                    Expires = DateTime.UtcNow.AddMinutes(90)
                };

                modelToInsert.PathUrl = _awsS3Client.GetPreSignedURL(request);

                result.SingleData = modelToInsert;
            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error guardando el archivo. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(model);
                _logRepository.InsertLog("S3StorageService.UploadFileAsync", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager<SavedFileDTO>> UploadFileFromBase64Async(UploadFileFromBase64DTO model)
        {


            var result = new ResponseManager<SavedFileDTO>();
            try
            {
                string fileName = $"Base64File-{DateTime.UtcNow.ToString("hh:mm:ss")}.{model.Extension}";

                byte[] fileByteArray = Convert.FromBase64String(model.Base64);

                MemoryStream memoryStream = new MemoryStream(fileByteArray);

                // Crea un FormFile con el MemoryStream
                IFormFile formFile = new FormFile(memoryStream, 0, fileByteArray.Length, fileName, fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/octet-stream"
                };

                var newModel = new UploadFileDTO
                {
                    File = formFile,
                    CategoryId = model.CategoryId,
                    StorageTypeId = model.StorageTypeId,
                    CompanyId = model.CompanyId,
                    UserId = model.UserId
                };

                var response = await UploadFileAsync(newModel);

                result = response;
            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error guardando el archivo. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(model);
                _logRepository.InsertLog("S3StorageService.UploadFileFromBase64Async", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager> DeleteFileAsync(ViewGetSavedFile file, int userId)
        {
            var result = new ResponseManager();



            try
            {


                if (string.IsNullOrEmpty(file?.RouteSaved))
                {
                    result.Errors.Add("El documento solicitado no posee una ruta asignada para poder ser eliminado. Favor de Comunicarse con un administrador");
                    return result;
                }

                await _context.Database.BeginTransactionAsync();

                var responseInactiveData = await _documentRepository.InactiveSavedFile(file.FileId, userId);
                if (!responseInactiveData.Succeded)
                {
                    result.Errors.AddRange(responseInactiveData.Errors);
                    await _context.Database.RollbackTransactionAsync();
                    return result;
                }

                var sendResponse = await SendDeleteRequestAsync(file.RouteSaved);

                if (!sendResponse)
                {
                    result.Errors.Add("No se ha podido envíar la petición de eliminación del documento. Favor de Comunicarse con un administrador");
                    await _context.Database.RollbackTransactionAsync();
                    return result;
                }

                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                if (_context.Database.CurrentTransaction != null)
                {
                    await _context.Database.RollbackTransactionAsync();
                }

                result.Errors.Add("Ha ocurrido un error eliminando el documento. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(new { file, userId });
                _logRepository.InsertLog("S3StorageService.DeleteFileAsync", parameter, msg);
            }

            return result;
        }

        private async Task<bool> SendDeleteRequestAsync(string fileName)
        {
            try
            {

                var existsFile = await CheckIfFileExistsAsync(fileName);

                if (!existsFile)
                {
                    return true;
                }


                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                await _awsS3Client.DeleteObjectAsync(deleteObjectRequest);

            }
            catch (Exception ex)
            {
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(new { fileName });
                _logRepository.InsertLog("S3StorageService.SendDeleteRequestAsync", parameter, msg);

                return false;
            }

            return true;
        }

        private async Task<bool> CheckIfFileExistsAsync(string fileName)
        {
            try
            {
                GetObjectMetadataRequest request = new GetObjectMetadataRequest()
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                var response = await _awsS3Client.GetObjectMetadataAsync(request);

                return true;
            }
            catch (Exception ex)
            {
                if (ex is AmazonS3Exception awsEx)
                {
                    if (awsEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return false;
                }

                throw;
            }
        }

    }
}
