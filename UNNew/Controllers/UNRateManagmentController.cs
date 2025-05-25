using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.UNRateDto;
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
    public class UNRateManagmentController : ControllerBase
    {
        private readonly IUNRateManagmentService _unRateService;
        private readonly IMapper _mapper;

        public UNRateManagmentController(IUNRateManagmentService unRateService, IMapper mapper)
        {
            _unRateService = unRateService;
            _mapper = mapper;
        }

        // Create UN Rate
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateUNRate([FromBody] CreateUNRateDto createUNRateDto)
        {
            var response = await _unRateService.CreateUnRateAsync(createUNRateDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetUNRateById), new { id = response.Data }, response);
        }

        // Get UN Rate By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UNRateDto>>> GetUNRateById(int id)
        {
            var response = await _unRateService.GetUnRateByIdAsync(id);
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("Record not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }

        // Get All UN Rates
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UNRateDto>>>> GetAllUNRates()
        {
            var response = await _unRateService.GetAllUnRatesAsync();
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("No records found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }

        // Update UN Rate
        [HttpPut("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateUNRate([FromBody] UNRateDto updateUNRateDto)
        {
            var response = await _unRateService.UpdateUnRateAsync(updateUNRateDto);
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("Record not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }

        // Delete UN Rate
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<string>>> DeleteUNRate(int id)
        {
            var response = await _unRateService.DeleteUnRateAsync(id);
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("Record not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}

