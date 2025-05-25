using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.ContractDtos;
using UNNew.DTOS.PoNumberDto;
using UNNew.Filters;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Controllers
{
   // [Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class ContractManagmentController : ControllerBase
    {
        private readonly IContractManagmentService _contractService;
        private readonly IMapper _mapper;

        public ContractManagmentController(IContractManagmentService contractService, IMapper mapper)
        {
            _contractService = contractService;
            _mapper = mapper;
        }

        // Get All Contracts
        [HttpPost("GetAll/{Id}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ContractDto>>>> GetAllContracts([FromBody]FilterModel filterModel,int? Id)
        {
            var response = await _contractService.GetAllContractsAsync(filterModel,Id);
            return Ok(response); // استخدم ApiResponse مباشرة
        }
        [HttpGet("GetAllPoNumber")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GetAllPoNumberDto>>>> GetAllPoNumber()
        {
            var response = await _contractService.GetAllPoNumber();
            if (response.Data == null || !response.Data.Any())
            {
                return NotFound(new ApiResponse<IEnumerable<GetAllPoNumberDto>>("No PoNumber found", null, new List<string> { "No data available" }));
            }

            return Ok(response); // استخدم ApiResponse مباشرة
        }

        // Create Contract
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateContract([FromBody] AddContractDto addContractDto)
        {
            var response = await _contractService.CreateContract(addContractDto);
            if (!response.Success)
            {
                return BadRequest(response); // استخدم ApiResponse مباشرة
            }

            return CreatedAtAction(nameof(GetContractById), new { id = response.Data }, response); // استخدم GetContractById بدلاً من GetAllContracts
        }

        // Update Contract
        [HttpPut("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateContract(int Id, [FromBody] UpdateContractDto updateContractDto)
        {
            var response = await _contractService.UpdateContractAsync(Id,updateContractDto);
            if (!response.Success)
            {
                return BadRequest(response); // استخدم ApiResponse مباشرة
            }

            return Ok(response); // استخدم ApiResponse مباشرة
        }

        // Delete Contract
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteContract(int id)
        {
            var response = await _contractService.DeleteContractAsync(id);
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

        // Get By Id Contract
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GetByIdContractDto>>> GetContractById(int id)
        {
            var response = await _contractService.GetContractByIdAsync(id);
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
        [HttpGet("GetActiveContractByEmployeeId/{id}")]
        public async Task<ActionResult<ApiResponse<GetByIdContractDto>>> GetActiveContractByEmployeeId(int id)
        {
            var response = await _contractService.GetContractByEmployeeIdAsync(id);
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
        // Cancel Contract
        [HttpPost("CancelContractAsync/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> CancelContractAsync(int id, BeforCancelContractDto beforCancelContractDto)
        {
            var response = await _contractService.CancelContractAsync(id, beforCancelContractDto);
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
        [HttpGet("UpdateEndContractBeforCancel/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateEndContractBeforCancel(int id,BeforCancelContractDto beforCancelContractDto)
        {
            var response = await _contractService.UpdateEndContractBeforCancel(id,beforCancelContractDto);
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

