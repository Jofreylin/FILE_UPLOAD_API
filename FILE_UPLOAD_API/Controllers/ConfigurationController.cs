using FILE_UPLOAD_API.DTO;
using FILE_UPLOAD_API.Models;
using FILE_UPLOAD_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FILE_UPLOAD_API.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IDocumentRepository _repository;
        public ConfigurationController(IDocumentRepository repo)
        {
            _repository = repo;
        }

        [HttpGet("Categories")]
        public async Task<ActionResult<ResponseManager<ViewGetDocumentCategory>>> GetCategoriesList(int categoryId, int sectionId, string sectionKeyword, int companyId, int type)
        {
            var response = await _repository.GetCategoriesList( categoryId,  sectionId,  sectionKeyword,  companyId,  type);

            return Ok(response);
        }

        [HttpGet("Sections")]
        public async Task<ActionResult<ResponseManager<ViewGetDocumentCategory>>> GetSectionsList(int sectionId, string sectionKeyword, int companyId, int type)
        {
            var response = await _repository.GetSectionsList(sectionId, sectionKeyword, companyId, type);

            return Ok(response);
        }

        [HttpGet("StorageTypes")]
        public async Task<ActionResult<ResponseManager<ViewGetDocumentCategory>>> GetStorageTypesList(int typeId, int categoryId, int fileId, Guid fileName, int companyId, int type)
        {
            var response = await _repository.GetStorageTypesList(typeId, categoryId, fileId, fileName, companyId, type);

            return Ok(response);
        }
    }
}
