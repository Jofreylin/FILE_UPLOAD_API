using Azure;
using Microsoft.AspNetCore.Mvc;
using FILE_UPLOAD_API.DTO;
using FILE_UPLOAD_API.Services;
using FILE_UPLOAD_API.Utilities;
using FILE_UPLOAD_API.Repositories;
using FILE_UPLOAD_API.Models;
using System.IO;
using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace FILE_UPLOAD_API.Controllers
{
   
    [ApiController]
    [Route("api/[Controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly StorageServiceFactory _factory;
        private readonly IDocumentRepository _documentRepository;

        public DocumentController(StorageServiceFactory factory, IDocumentRepository documentRepository)
        {
            _factory = factory;
            _documentRepository = documentRepository;
        }

        [HttpGet("Download")]
        public async Task<ActionResult> DownloadFile(string fileName)
        {
            var response = new ResponseManager<MemoryStream>();

            MemoryStream memory;

            var extension = Path.GetExtension(fileName);

            if (!string.IsNullOrWhiteSpace(extension))
            {
                fileName = fileName.Replace(extension, "");
            }

            var fileNameGUID = new Guid(fileName);

            var documentResponse = await CheckIfFileisSavedInDatabase(fileNameGUID);
            if (!documentResponse.Succeded)
            {
                response.Errors.AddRange(documentResponse.Errors);
                return StatusCode(response?.StatusCode > 0 ? (int)response.StatusCode : 400, response);
            }

            var file = documentResponse.SingleData;

            var storageService = _factory.CreateService(file?.StorageTypeId);

            response = await storageService.DownloadFileAsync(file);

            if (!response.Succeded)
            {
                return StatusCode(400,response);
            }

            if(response.SingleData is null)
            {
                response.Errors.Add("El archivo solicitado no pudo ser procesado. Favor de comunicarse con un administrador.");
                return StatusCode(400, response);
            }

            memory = response.SingleData;

            string completeFileName = $"{file?.FileName}{file?.OriginalFileExtension}";

            string? mimeType = Utility.GetContentType(completeFileName);

            if (mimeType is null)
            {
                response.Errors.Add("Ha ocurrido un error tratando de descargar el archivo. Extensión irreconocible, Favor de comunicarse con un administrador.");
                return BadRequest(response);
            }

            return File(memory?.ToArray(), mimeType, completeFileName);
        }

       
        [HttpGet]
        public async Task<ActionResult<ResponseManager<string>>> GetFilePath(Guid fileName)
        {
            var response = new ResponseManager<string>();

            var documentResponse = await CheckIfFileisSavedInDatabase(fileName);
            if (!documentResponse.Succeded)
            {
                response.Errors.AddRange(documentResponse.Errors);
                return Ok(response);
            }

            var file = documentResponse.SingleData;

            var storageService = _factory.CreateService(file?.StorageTypeId);

            response = await storageService.GetFilePathAsync(file);

            return Ok(response);
        }

       
        [HttpPost("GetPathsByList")]
        public async Task<ActionResult<ResponseManager<ReturnFilePath>>> GetFilePathsList(Guid[] fileNames)
        {
            var response = new ResponseManager<ReturnFilePath>();

            var list = new List<ReturnFilePath>();
            var files = new List<ViewGetSavedFile>();

            foreach (var name in fileNames)
            {
                var fileToReturn = new ReturnFilePath
                {
                    FileName = name
                };

                var documentResponse = await CheckIfFileisSavedInDatabase(name);
                if (!documentResponse.Succeded)
                {
                    fileToReturn.Found = false;
                    fileToReturn.ErrorMessage = documentResponse.Errors[0];
                    list.Add(fileToReturn);
                }
                else
                {
                    files.Add(documentResponse?.SingleData);
                }

                
            }

            var tasks = files.Select(file => Task.Run(async () =>
            {
                var fileToReturn = new ReturnFilePath
                {
                    FileName = file.FileName
                };

                var storageService = _factory.CreateService(file?.StorageTypeId);

                var storageResponse = await storageService.GetFilePathAsync(file);
                if (!storageResponse.Succeded)
                {
                    fileToReturn.Found = false;
                    fileToReturn.ErrorMessage = storageResponse.Errors[0];
                    list.Add(fileToReturn);
                    return;
                }

                fileToReturn.Path = storageResponse.SingleData;
                fileToReturn.Found = true;

                list.Add(fileToReturn);

            }));

            await Task.WhenAll(tasks);


            response.DataList = list;

            return Ok(response);
        }


        [HttpGet("GetBase64")]
        public async Task<ActionResult<ResponseManager<string>>> GetBase64File(Guid fileName)
        {
            var response = new ResponseManager<string>();

            var documentResponse = await CheckIfFileisSavedInDatabase(fileName);
            if (!documentResponse.Succeded)
            {
                response.Errors.AddRange(documentResponse.Errors);
                return Ok(response);
            }

            var file = documentResponse.SingleData;

            var storageService = _factory.CreateService(file?.StorageTypeId);

            response = await storageService.GetFileBase64Async(file);

            return Ok(response);
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<ActionResult<ResponseManager<SavedFileDTO>>> Post([FromForm] UploadFileDTO model)
        {
            var storageService = _factory.CreateService(model.StorageTypeId);

            var response = await storageService.UploadFileAsync(model);

            return Ok(response);
        }

        [HttpPost("FromBase64")]
        public async Task<ActionResult<ResponseManager<SavedFileDTO>>> PostFromBase64(UploadFileFromBase64DTO model)
        {
            var storageService = _factory.CreateService(model.StorageTypeId);

            var response = await storageService.UploadFileFromBase64Async(model);

            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<ResponseManager>> DeleteFile(Guid fileName, int userId)
        {
            var response = new ResponseManager();

            var documentResponse = await CheckIfFileisSavedInDatabase(fileName);
            if (!documentResponse.Succeded)
            {
                response.Errors.AddRange(documentResponse.Errors);
                return Ok(response);
            }

            var file = documentResponse.SingleData;

            var storageService = _factory.CreateService(file?.StorageTypeId);

            response = await storageService.DeleteFileAsync(file, userId);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("ViewDocument")]
        public async Task<ActionResult> ViewDocument(string fileName)
        {
            var response = new ResponseManager<MemoryStream>();

            MemoryStream memory;

            var extension = Path.GetExtension(fileName);

            if (!string.IsNullOrWhiteSpace(extension))
            {
                fileName = fileName.Replace(extension, "");
            }

            var fileNameGUID = new Guid(fileName);

            var documentResponse = await CheckIfFileisSavedInDatabase(fileNameGUID);
            if (!documentResponse.Succeded)
            {
                response.Errors.AddRange(documentResponse.Errors);
                return StatusCode(response?.StatusCode > 0 ? (int)response.StatusCode : 500, response);
            }

            var file = documentResponse.SingleData;

            var storageService = _factory.CreateService(file?.StorageTypeId);

            response = await storageService.DownloadFileAsync(file);

            if (!response.Succeded)
            {
                return StatusCode(400,response);
            }

            if (response.SingleData is null)
            {
                response.Errors.Add("El archivo solicitado no pudo ser procesado. Favor de comunicarse con un administrador.");
                return StatusCode(400, response);
            }

            memory = response.SingleData;

            string completeFileName = $"{file?.FileName}{file?.OriginalFileExtension}";
            string? mimeType = Utility.GetContentType(completeFileName);

            if (mimeType is null)
            {
                response.Errors.Add("Ha ocurrido un error tratando de descargar el archivo. Extensión irreconocible, Favor de comunicarse con un administrador.");
                return BadRequest(response);
            }

            return File(memory.ToArray(), mimeType, enableRangeProcessing: true);
        }

        [NonAction]
        private async Task<ResponseManager<ViewGetSavedFile>> CheckIfFileisSavedInDatabase(Guid fileName)
        {
            var response = new ResponseManager<ViewGetSavedFile>();

            var documentResponse = await _documentRepository.GetSavedFilesList(0, fileName, 0, 0, 0, 0, type: 0);
            if (!documentResponse.Succeded)
            {
                response.Errors.AddRange(documentResponse.Errors);
                return response;
            }

            var file = documentResponse.DataList.FirstOrDefault();

            if (documentResponse.DataList.Count() > 1)
            {
                response.Errors.Add("Se ha encontrado mas de un documento con ese identificador, no es posible realizar la operación");
                response.StatusCode = (int?)HttpStatusCode.BadRequest;
                return response;
            }

            if (file is null)
            {
                response.Errors.Add("No se ha encontrado registro del documento solicitado");
                response.StatusCode = (int?)HttpStatusCode.NotFound;
                return response;
            }

            response.SingleData = file;

            return response;
        }

    }
}