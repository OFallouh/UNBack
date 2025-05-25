using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.UNEmployeeDtos;
using UNNew.Response;
using UNNew.Repository.Interfaces;
using UNNew.Filters;
using Azure.Core;
using UNNew.Repository.Services;
using UNNew.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace UNNew.Controllers
{
   // [Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class UNEmployeeManagmentController : ControllerBase
    {
        private readonly IUNEmployeeManagmentService _UNEmployeeManagmentService;
        private readonly IMapper _mapper;

        public UNEmployeeManagmentController(IUNEmployeeManagmentService uNEmployeeManagmentService, IMapper mapper)
        {
            _UNEmployeeManagmentService = uNEmployeeManagmentService;
            _mapper = mapper;
        }

        // Get All Employees
        [HttpPost("GetAllUNEmployee")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UNEmployeeDto>>>> GetAll(
     [FromBody] FilterModel filterModel, [FromQuery] string? PoNumber)
        {
            // استخدم await هنا للتأكد من استلام البيانات
            var response = await _UNEmployeeManagmentService.GetAllEmployeesAsync(filterModel, PoNumber);

            // تحقق من وجود أخطاء
            if (response?.Errors?.Any() == true)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }



        // Create Employee
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> Create([FromBody] CreateUNEmployeeDto createUNEmployeeDto)
        {
            var response = await _UNEmployeeManagmentService.CreateUNEmployeeAsync(createUNEmployeeDto);

            if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
            {
                return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
            }

            return CreatedAtAction(nameof(GetAll), new { id = response.Data }, response); // Use response.RefNo or another identifier if needed
        }

        //Update Employee
       [HttpPut("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] UpdateUNEmployeeDto updateUNEmployeeDto,int Id)
        {
            var response = await _UNEmployeeManagmentService.UpdateUNEmployeeAsync(updateUNEmployeeDto, Id);
            if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
            {
                return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
            }

            return Ok(new ApiResponse<string>("Employee updated successfully", response.Data, null));
        }

        // Delete Employee
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var response = await _UNEmployeeManagmentService.DeleteUNEmployeeAsync(id);
            if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
            {
                return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
            }

            return Ok(new ApiResponse<string>("Employee deleted successfully", null, null));
        }

        // Disable Employee (Soft Delete)
        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Disable(int id)
        {
            var response = await _UNEmployeeManagmentService.DisableUNEmployeeAsync(id);
            if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
            {
                return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
            }

            return Ok(new ApiResponse<string>("Employee disabled successfully", null, null));
        }
        // GetById Employee 
        [HttpGet("{refNo}")]
        public async Task<ActionResult<ApiResponse<UNEmployeeDto>>> GetByIdEmployee(int refNo)
        {
            try
            {
                var response = await _UNEmployeeManagmentService.GetByIdEmployeeAsync(refNo);
                if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
                {
                    return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
                }

                return Ok(response);  // Return the response when employee is found
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UNEmployeeDto>("Error occurred while retrieving employee", null, new List<string> { ex.Message }));
            }
        }

        [HttpPost("upload/{Id}")]
        public async Task<IActionResult> UploadFiles(int Id, [FromForm] List<IFormFile> files)
        {
            var response = await _UNEmployeeManagmentService.UploadFilesAsync(Id, files);
            if (response.Errors.Any())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("files/{Id}")]
        public async Task<IActionResult> GetEmployeeFiles(int Id)
        {
            var response = await _UNEmployeeManagmentService.GetEmployeeFilesAsync(Id);
            if (response.Errors.Any())
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet("download/{Id}/{fileName}")]
        public async Task<IActionResult> DownloadFile(int Id, string fileName)
        {
            var response = await _UNEmployeeManagmentService.DownloadFileAsync(Id, fileName);
            if (response.Errors.Any())
                return NotFound(response);

            return File(response.Data.content, response.Data.contentType, response.Data.fileName);
        }

        [HttpDelete("delete/{Id}/{fileName}")]
        public async Task<IActionResult> DeleteFile(int Id, string fileName)
        {
            var response = await _UNEmployeeManagmentService.DeleteFileAsync(Id, fileName);
            if (response.Errors.Any())
                return BadRequest(response.Errors);

            return Ok(response);
        }

        [HttpDelete("deleteAll/{Id}")]
        public async Task<IActionResult> DeleteAllFiles(int Id)
        {
            var response = await _UNEmployeeManagmentService.DeleteAllFilesAsync(Id);
            if (response.Errors.Any())
                return BadRequest(response);

            return Ok(response);
        }
    }
}

