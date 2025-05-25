using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.LaptopDto;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Repository.Services;
using UNNew.Response;

namespace UNNew.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class LaptopRentManagmentController :ControllerBase
    {
        private readonly ILaptopRentManagmentService _laptopService;
        private readonly IMapper _mapper;

        public LaptopRentManagmentController(ILaptopRentManagmentService laptopRentManagmentService, IMapper mapper)
        {
            _laptopService = laptopRentManagmentService;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateLaptopRent([FromBody] CreateLaptopRentDto dto)
        {
            var response = await _laptopService.CreateLaptopRentAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Get All Laptop Rents
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<LaptopRentDto>>>> GetAllLaptopRents()
        {
            var response = await _laptopService.GetAllLaptopRentsAsync();
            return response.Success ? Ok(response) : NotFound(response);
        }

        

        // Update Laptop Rent
        [HttpPut]
        public async Task<ActionResult<ApiResponse<string>>> UpdateLaptopRent([FromBody] UpdateLaptopRentDto dto)
        {
            var response = await _laptopService.UpdateLaptopRentAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }
       
      

        // Delete Laptop Rent
        [HttpDelete("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteLaptopRent(int Id)
        {
            var response = await _laptopService.DeleteLaptopRentAsync(Id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
