using FILE_UPLOAD_API.Context;
using FILE_UPLOAD_API.DTO;
using FILE_UPLOAD_API.Models;
using FILE_UPLOAD_API.Repositories;
using FILE_UPLOAD_API.Utilities;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;
using System.ComponentModel.Design;
using Microsoft.Data.SqlClient;

namespace FILE_UPLOAD_API.Services
{
    public interface IDocumentRepository
    {
        Task<ResponseManager<ViewGetSavedFile>> GetSavedFilesList(int fileId, Guid fileName, int categoryId, int sectionId, int storageTypeId, int companyId, int type);
        Task<ResponseManager<ViewGetDocumentCategory>> GetCategoriesList(int categoryId, int sectionId, string sectionKeyword, int companyId, int type);
        Task<ResponseManager<ViewGetDocumentSection>> GetSectionsList(int sectionId, string sectionKeyword, int companyId, int type);
        Task<ResponseManager<ViewGetStorageType>> GetStorageTypesList(int typeId, int categoryId, int fileId, Guid fileName, int companyId, int type);
        Task<ResponseManager> InsertSavedFileData(SavedFileDTO model);
        Task<ResponseManager> InactiveSavedFile(int fileId, int? userId);
    }

    public class DocumentService : IDocumentRepository
    {
        private readonly IWebHostEnvironment Environment;
        private readonly DocApiContext _context;
        private readonly ILogRepository _logRepository;

        public DocumentService(IWebHostEnvironment env, DocApiContext context, ILogRepository logRepository)
        {
            this.Environment = env;
            _context = context;
            _logRepository = logRepository;
        }

        
        public async Task<ResponseManager<ViewGetSavedFile>> GetSavedFilesList(int fileId, Guid fileName, int categoryId, int sectionId, int storageTypeId, int companyId, int type)
        {
            var result = new ResponseManager<ViewGetSavedFile>();

            try
            {

                var list = await _context.ViewGetSavedFiles.FromSqlInterpolated(@$"
                                                  [XDMS].[Sp_GetSavedFiles]
	                                                @FileId = {fileId},
	                                                @FileName = {fileName},
	                                                @CategoryId = {categoryId},
	                                                @SectionId = {sectionId},
	                                                @StorageTypeId = {storageTypeId},
	                                                @CompanyId = {companyId},
	                                                @Type = {type}  
                ").ToListAsync();
                result.DataList = list;


            }catch(Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error tratando de consultar la lista de archivos guardados. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(new { fileId,  fileName,  categoryId,  sectionId,  storageTypeId,  companyId,  type });
                _logRepository.InsertLog("DocumentService.GetSavedFilesList", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager<ViewGetDocumentCategory>> GetCategoriesList(int categoryId, int sectionId, string sectionKeyword, int companyId, int type)
        {
            var result = new ResponseManager<ViewGetDocumentCategory>();

            try
            {

                var list = await _context.ViewGetDocumentCategories.FromSqlInterpolated(@$"
                                                  [XDMS].[Sp_GetDocumentCategories]
	                                                @CategoryId = {categoryId},
	                                                @SectionId = {sectionId},
	                                                @SectionKeyword = {sectionKeyword},
	                                                @CompanyId = {companyId},
	                                                @Type = {type}  
                ").ToListAsync();
                result.DataList = list;


            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error tratando de consultar la lista de categorias de los archivos. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(new {  categoryId, sectionId, sectionKeyword, companyId, type });
                _logRepository.InsertLog("DocumentService.GetCategoriesList", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager<ViewGetDocumentSection>> GetSectionsList(int sectionId, string sectionKeyword, int companyId, int type)
        {
            var result = new ResponseManager<ViewGetDocumentSection>();

            try
            {

                var list = await _context.ViewGetDocumentSections.FromSqlInterpolated(@$"
                                                  [XDMS].[Sp_GetDocumentSections]
	                                                @SectionId = {sectionId},
	                                                @SectionKeyword = {sectionKeyword},
	                                                @CompanyId = {companyId},
	                                                @Type = {type}  
                ").ToListAsync();
                result.DataList = list;


            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error tratando de consultar la lista de secciones de los archivos. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(new { sectionId, sectionKeyword, companyId, type });
                _logRepository.InsertLog("DocumentService.GetSectionsList", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager<ViewGetStorageType>> GetStorageTypesList(int typeId, int categoryId, int fileId, Guid fileName, int companyId, int type)
        {
            var result = new ResponseManager<ViewGetStorageType>();

            try
            {

                var list = await _context.ViewGetStorageTypes.FromSqlInterpolated(@$"
                                                  [XDMS].[Sp_GetStorageTypes]
	                                                @TypeId = {typeId},
	                                                @CategoryId = {categoryId},
                                                    @FileId = {fileId},
                                                    @FileName = {fileName}
	                                                @CompanyId = {companyId},
	                                                @Type = {type}  
                ").ToListAsync();
                result.DataList = list;


            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error consultando la lista de tipos de almacenamientos. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(new { typeId, categoryId, fileId, fileName, companyId, type });
                _logRepository.InsertLog("DocumentService.GetSectionsList", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager> InsertSavedFileData(SavedFileDTO model)
        {
            var result = new ResponseManager();

            try
            {
                var output = new SqlParameter("@Output", SqlDbType.Int) { Direction = ParameterDirection.Output }; 

                await _context.Database.ExecuteSqlInterpolatedAsync(@$"
                                                  [XDMS].[Sp_SetSavedFiles]
	                                                @FileId = {model.FileId} 
	                                                ,@FileName = {model.FileName} 
	                                                ,@OriginalFileName = {model.OriginalFileName} 
	                                                ,@OriginalFileExtension = {model.OriginalFileExtension} 
	                                                ,@DocumentCategoryId = {model.DocumentCategoryId}  
	                                                ,@RouteSaved = {model.RouteSaved} 
	                                                ,@StorageTypeId = {model.StorageTypeId} 
	                                                ,@CompanyId = {model.CompanyId} 
	                                                ,@UserId = {model.UserId} 
	                                                ,@Operation = {Utility.DbOperations.Insert} 
	                                                ,@Output = {output} out
                ");

                result.Identity = (int)output.Value;


            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error insertando la información del archivo enviado. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(model);
                _logRepository.InsertLog("DocumentService.InsertSavedFileData", parameter, msg);
            }

            return result;
        }

        public async Task<ResponseManager> InactiveSavedFile(int fileId, int? userId)
        {
            var result = new ResponseManager();

            try
            {
                var output = new SqlParameter("@Output", SqlDbType.Int) { Direction = ParameterDirection.Output };

                await _context.Database.ExecuteSqlInterpolatedAsync(@$"
                                                  [XDMS].[Sp_SetSavedFiles]
	                                                @FileId = {fileId} 
	                                                ,@FileName = NULL
	                                                ,@OriginalFileName = NULL
	                                                ,@OriginalFileExtension = NULL
	                                                ,@DocumentCategoryId = NULL
	                                                ,@RouteSaved = NULL
	                                                ,@StorageTypeId = NULL
	                                                ,@CompanyId = NULL
	                                                ,@UserId = {userId}
	                                                ,@Operation = {Utility.DbOperations.Delete} 
	                                                ,@Output = {output} out
                ");

                result.Identity = (int)output.Value;


            }
            catch (Exception ex)
            {
                result.Errors.Add("Ha ocurrido un error inactivando la información del archivo enviado. Consulte un administrador");
                string msg = $"{ex?.Message} | {ex?.InnerException}";
                var parameter = JsonConvert.SerializeObject(new {fileId, userId});
                _logRepository.InsertLog("DocumentService.InactiveSavedFile", parameter, msg);
            }

            return result;
        }


        
    }
}
