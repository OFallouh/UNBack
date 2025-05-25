using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.CurrencyDtos;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyManagmentController : ControllerBase
    {
        private readonly ICurrencyManagmentService _currencyService;

        public CurrencyManagmentController(ICurrencyManagmentService currencyService)
        {
            _currencyService = currencyService;
        }

        // Create a new Currency
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateCurrency([FromBody] CreateCurrencyDto createCurrencyDto)
        {
            var response = await _currencyService.CreateCurrencyAsync(createCurrencyDto);
            if (response.Data != null)
            {
                return CreatedAtAction(nameof(GetCurrencyById), new { id = response.Data }, response);
            }

            return BadRequest(response);
        }

        // Update an existing Currency
        [HttpPut("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateCurrency([FromBody] UpdateCurrencyDto updateCurrencyDto)
        {
            var response = await _currencyService.UpdateCurrencyAsync(updateCurrencyDto);
            if (response.Data != null)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Get Currency by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CurrencyDto>>> GetCurrencyById(int id)
        {
            var response = await _currencyService.GetCurrencyByIdAsync(id);
            if (response.Data != null)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        // Get All Currencies
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CurrencyDto>>>> GetAllCurrencies()
        {
            var response = await _currencyService.GetAllCurrenciesAsync();
            if (response.Data != null && response.Data.Any())
            {
                return Ok(response);
            }

            return NoContent();
        }

        // Delete a Currency by ID
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCurrency(int id)
        {
            var response = await _currencyService.DeleteCurrencyAsync(id);
            if (response.Data == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}



