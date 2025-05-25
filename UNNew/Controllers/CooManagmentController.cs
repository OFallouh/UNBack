using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.CooDtos;
using UNNew.DTOS.UNEmployeeDtos;
using UNNew.Filters;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Repository.Services;
using UNNew.Response;

namespace UNNew.Controllers
{
    //[Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class CooManagmentController : ControllerBase
    {

        private readonly ICooManagmentService _cooManagmentService;

        public CooManagmentController(ICooManagmentService cooManagmentService)
        {
            _cooManagmentService = cooManagmentService;
        }

        // CreateCoo
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateCooAsync([FromBody] CreateCooDto createCooDto)
        {
            var response = await _cooManagmentService.CreateCooAsync(createCooDto);
            if (response.Data == null)
            {
                return BadRequest(new ApiResponse<string>("Error creating COO", null, response.Errors));
            }

            return CreatedAtAction(nameof(GetCooById), new { id = response.Data }, response);
        }

        // UpdateCoo
        [HttpPut("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateCooAsync([FromBody] UpdateCooDto updateCooDto, int Id)
        {
            var response = await _cooManagmentService.UpdateCooAsync(updateCooDto,Id);
            if (response.Data == null)
            {
                return BadRequest(new ApiResponse<string>("Error updating COO", null, response.Errors));
            }

            return Ok(new ApiResponse<string>("COO updated successfully", response.Data, null));
        }

        // GetCooById
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GetByIdDto>>> GetCooById(int id)
        {
            try
            {
                var response = await _cooManagmentService.GetCooByIdAsync(id);
                if (response.Data == null)
                {
                    return NotFound(new ApiResponse<GetByIdDto>("COO not found", null, new List<string> { "No COO found with the provided ID." }));
                }

                return Ok(response);  // Return the response when COO is found
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<GetByIdDto>("Error occurred while retrieving COO", null, new List<string> { ex.Message }));
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteContract(int id)
        {
            var response = await _cooManagmentService.DeleteCooAsync(id);
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("COO with the provided ID not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response); // استخدم ApiResponse مباشرة
        }
       

        // GetAllCoos
        [HttpPost("GetAll")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CooDto>>>> GetAllCoosAsync(FilterModel filterModel)
        {
            var response = await _cooManagmentService.GetAllCoosAsync(filterModel);
            if (response.Data == null || !response.Data.Any())
            {
                return NotFound(new ApiResponse<IEnumerable<CooDto>>("No coos found", null, new List<string> { "No data available" }));
            }

            return Ok(response);
        }
        [HttpPost("GetAllCooWithoutPagination")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GetAllCooWithoutPaginationDto>>>> GetAllCooWithoutPagination( )
        {
            var response = await _cooManagmentService.GetAllCooWithoutPagination();
            if (response.Data == null || !response.Data.Any())
            {
                return NotFound(new ApiResponse<IEnumerable<GetAllCooWithoutPaginationDto>>("No coos found", null, new List<string> { "No data available" }));
            }

            return Ok(new ApiResponse<IEnumerable<GetAllCooWithoutPaginationDto>>("Coos fetched successfully", response.Data, null));
        }

        [HttpPost("upload/{Id}")]
        public async Task<IActionResult> UploadFiles(int Id, [FromForm] List<IFormFile> files)
        {
            var response = await _cooManagmentService.UploadFilesForCooAsync(Id, files);
            if (response.Errors.Any())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("files/{Id}")]
        public async Task<IActionResult> GetCooFilesAsync(int Id)
        {
            var response = await _cooManagmentService.GetCooFilesAsync(Id);
            if (response.Errors.Any())
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet("download/{Id}/{fileName}")]
        public async Task<IActionResult> DownloadFile(int Id, string fileName)
        {
            var response = await _cooManagmentService.DownloadCooFileAsync(Id, fileName);
            if (response.Errors.Any())
                return NotFound(response);

            return File(response.Data.content, response.Data.contentType, response.Data.fileName);
        }

        [HttpDelete("delete/{Id}/{fileName}")]
        public async Task<IActionResult> DeleteFile(int Id, string fileName)
        {
            var response = await _cooManagmentService.DeleteCooFileAsync(Id, fileName);
            if (response.Errors.Any())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("deleteAll/{Id}")]
        public async Task<IActionResult> DeleteAllFiles(int Id)
        {
            var response = await _cooManagmentService.DeleteAllCooFilesAsync(Id);
            if (response.Errors.Any())
                return BadRequest(response);

            return Ok(response);
        }

    }
}
