using Amazon.S3;
using Amazon.S3.Model;
using FILE_UPLOAD_API.Context;
using FILE_UPLOAD_API.DTO;
using FILE_UPLOAD_API.Models;
using FILE_UPLOAD_API.Repositories;
using FILE_UPLOAD_API.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System.IO;

namespace FILE_UPLOAD_API.Services
{
    public class LocalStorageService : IStorageRepository
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogRepository _logRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly DocApiContext _context;
        private readonly string apiBaseUrl;

        public LocalStorageService(ILogRepository logRepository, IDocumentRepository documentRepository,  DocApiContext context, IConfiguration configuration, IWebHostEnvironment env)
        {

            _logRepository = logRepository;
            _documentRepository = documentRepository;
            _context = context;
            _env = env;

            this.apiBaseUrl = configuration?.GetValue<string>("ApiBaseUrl");
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

                var memory = new MemoryStream();

                using (var stream = new FileStream(file.RouteSaved, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;

                result.SingleData = memory;

            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error descargando el documento. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(file);
                _logRepository.InsertLog("LocalStorageService.DownloadFileAsync", parameter, msg);
            }

            return result;
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
                _logRepository.InsertLog("LocalStorageService.GetFileBase64Async", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager<string>> GetFilePathAsync(ViewGetSavedFile file)
        {
            var result = new ResponseManager<string>();

            try
            {

                if (string.IsNullOrEmpty(apiBaseUrl))
                {
                    result.Errors.Add("Este servicio para ver el documento solicitado aún no ha sido configurado. Favor de Comunicarse con un administrador");
                    return result;
                }

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

                //example: https://api-doc.sigei.do

                string fileName = $"{file?.FileName}{file?.OriginalFileExtension}";
                var url = Utility.GetUrlToViewFile(apiBaseUrl, fileName);

                result.SingleData = url;

            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error consultando la lista de documentos. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(file);
                _logRepository.InsertLog("LocalStorageService.GetFilePathAsync", parameter, msg);
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
                _logRepository.InsertLog("LocalStorageService.GetFilePathListAsync", parameter, msg);
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

                if(model.CompanyId <= 0)
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

                string newFileName = $"{fileNameGUID}{originalExtension}";
                string folderToSave = $"{_env.WebRootPath}/{Constants.FolderSaveFile}/{model.CompanyId}/{category.Directory}";
                string savedInPath = $"{folderToSave}/{newFileName}";

                var modelToInsert = new SavedFileDTO
                {
                    FileName = fileNameGUID,
                    OriginalFileExtension = originalExtension,
                    OriginalFileName = originalFileName,
                    RouteSaved = savedInPath,
                    StorageTypeId = model.StorageTypeId,
                    DocumentCategoryId = model.CategoryId,
                    CompanyId = model.CompanyId,
                    UserId = model.UserId
                };

                if (!Directory.Exists(folderToSave)) Directory.CreateDirectory(folderToSave);

                using var stream = new FileStream(savedInPath, FileMode.Create); 
                await file.CopyToAsync(stream);

                var responseSaveFileRecord = await _documentRepository.InsertSavedFileData(modelToInsert);
                if (!responseSaveFileRecord.Succeded)
                {
                    result.Errors.AddRange(responseSaveFileRecord.Errors);

                    var sendResponse = SendDeleteRequestAsync(savedInPath);

                    return result;
                }

                modelToInsert.PathUrl = Utility.GetUrlToViewFile(apiBaseUrl, $"{modelToInsert.FileName}{modelToInsert.OriginalFileExtension}");

                result.SingleData = modelToInsert;
            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error guardando el archivo. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(model);
                _logRepository.InsertLog("LocalStorageService.UploadFileAsync", parameter, msg);
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
                _logRepository.InsertLog("LocalStorageService.UploadFileFromBase64Async", parameter, msg);
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
                _logRepository.InsertLog("LocalStorageService.DeleteFileAsync", parameter, msg);
            }

            return result;
        }

        private async Task<bool> SendDeleteRequestAsync(string filePath)
        {
            try
            {

                var existsFile = await CheckIfFileExistsAsync(filePath);

                if (!existsFile)
                {
                    return true;
                }


                await Task.Run(() => File.Delete(filePath));

            }
            catch (Exception ex)
            {
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(new { filePath });
                _logRepository.InsertLog("LocalStorageService.SendDeleteRequestAsync", parameter, msg);

                return false;
            }

            return true;
        }

        public async Task<bool> CheckIfFileExistsAsync(string filePath)
        {
            return await Task.Run(() => File.Exists(filePath));
        }
    }
}
