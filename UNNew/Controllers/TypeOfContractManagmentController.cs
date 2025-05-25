using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.TypeOfContractDtos;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Controllers
{
    //[Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class TypeOfContractManagmentController : ControllerBase
    {
        private readonly ITypeOfContractManagmentService _typeOfContractService;

        public TypeOfContractManagmentController(ITypeOfContractManagmentService typeOfContractService)
        {
            _typeOfContractService = typeOfContractService;
        }

        // Create a new TypeOfContract
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateTypeOfContract([FromBody] CreateTypeOfContractDto createTypeOfContractDto)
        {
            var response = await _typeOfContractService.CreateTypeOfContractAsync(createTypeOfContractDto);
            if (response.Data != null)
            {
                return CreatedAtAction(nameof(GetTypeOfContractById), new { id = response.Data }, response);
            }

            return BadRequest(response);
        }

        // Update an existing TypeOfContract
        [HttpPut("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateTypeOfContract([FromBody] UpdateTypeOfContractDto updateTypeOfContractDto)
        {
            var response = await _typeOfContractService.UpdateTypeOfContractAsync(updateTypeOfContractDto);
            if (response.Data != null)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Get TypeOfContract by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TypeOfContractDto>>> GetTypeOfContractById(int id)
        {
            var response = await _typeOfContractService.GetTypeOfContractByIdAsync(id);
            if (response.Data != null)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        // Get all TypeOfContracts
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TypeOfContractDto>>>> GetAllTypeOfContracts()
        {
            var response = await _typeOfContractService.GetAllTypeOfContractsAsync();
            if (response.Data != null && response.Data.Any())
            {
                return Ok(response);
            }

            return NoContent();
        }

        // Delete a TypeOfContract by ID
       
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteTypeOfContract(int id)
        {
            var response = await _typeOfContractService.DeleteTypeOfContractAsync(id);
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("Contract with the provided ID not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response); // استخدم ApiResponse مباشرة
        }
    }
}



