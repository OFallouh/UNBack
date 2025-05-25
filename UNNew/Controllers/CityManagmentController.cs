using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.CityDtos;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class CityManagmentController : ControllerBase
    {
        private readonly ICityManagmentService _cityService;
        private readonly IMapper _mapper;

        public CityManagmentController(ICityManagmentService cityService, IMapper mapper)
        {
            _cityService = cityService;
            _mapper = mapper;
        }

        // Get All Cities
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CityDto>>>> GetAllCities()
        {
            var response = await _cityService.GetAllCitiesAsync();
            if (response.Data == null || !response.Data.Any())
            {
                return NotFound(new ApiResponse<IEnumerable<CityDto>>("No cities found", null, new List<string> { "No data available" }));
            }

            return Ok(response); // استخدم ApiResponse مباشرة
        }

        // Create City
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateCity([FromBody] CreateCityDto createCityDto)
        {
            var response = await _cityService.CreateCityAsync(createCityDto);
            if (!response.Success)
            {
                return BadRequest(response); // استخدم ApiResponse مباشرة
            }

            return CreatedAtAction(nameof(GetCityById), new { id = response.Data }, response); // استخدم GetCityById بدلاً من GetAllCities
        }

        // Update City
        [HttpPut("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateCity([FromBody] UpdateCityDto updateCityDto)
        {
            var response = await _cityService.UpdateCityAsync(updateCityDto);
            if (!response.Success)
            {
                return BadRequest(response); // استخدم ApiResponse مباشرة
            }

            return Ok(response); // استخدم ApiResponse مباشرة
        }

        // Delete City
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCity(int id)
        {
            var response = await _cityService.DeleteCityAsync(id);
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("City with the provided ID not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response); // استخدم ApiResponse مباشرة
        }

        // Get By Id City
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CityDto>>> GetCityById(int id)
        {
            var response = await _cityService.GetCityByIdAsync(id);
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("City with the provided ID not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response); // استخدم ApiResponse مباشرة
        }
    }
}