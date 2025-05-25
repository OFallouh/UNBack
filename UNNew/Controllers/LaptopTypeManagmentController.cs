using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.LaptopDto;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Controllers
{
    //[Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class LaptopTypeManagmentController : ControllerBase
    {
        private readonly ILaptopRentManagmentService _laptopService;
        private readonly IMapper _mapper;

        public LaptopTypeManagmentController(ILaptopRentManagmentService laptopRentManagmentService, IMapper mapper)
        {
            _laptopService = laptopRentManagmentService;
            _mapper = mapper;
        }
        // Create Laptop Type
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateLaptopType([FromBody] CreateLaptopTypeDto dto)
        {
            var response = await _laptopService.CreateLaptopTypeAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Update Laptop Type
        [HttpPut("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateLaptopType([FromBody] UpdateLaptopTypeDto dto)
        {
            var response = await _laptopService.UpdateLaptopTypeAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }
        // Get Laptop Type by Id
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResponse<GetAllGetLaptopTypeDto>>> GetLaptopTypeById(int Id)
        {
            var response = await _laptopService.GetLaptopTypeByIdAsync(Id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        // Get All Laptop Types
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<GetLaptopTypeDto>>>> GetAllLaptopTypes()
        {
            var response = await _laptopService.GetAllLaptopTypesAsync();
            return response.Success ? Ok(response) : NotFound(response);
        }
        // Delete Laptop Type
        [HttpDelete("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteLaptopType(int Id)
        {
            var response = await _laptopService.DeleteLaptopTypeAsync(Id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
