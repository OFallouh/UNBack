using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.ContractDtos;
using UNNew.DTOS.DsaDto;
using UNNew.DTOS.InsuranceDtos;
using UNNew.DTOS.SalaryDtos;
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
    public class SalaryManagmentController :ControllerBase
    {
        private readonly ISalaryManagmentService _salaryManagmentService;
        private readonly IMapper _mapper;

        public SalaryManagmentController(ISalaryManagmentService salaryManagmentService, IMapper mapper)
        {
            _salaryManagmentService = salaryManagmentService;
            _mapper = mapper;
        }
        // Calculate Salary
        [HttpPut("CalculateSalary")]
        public async Task<ActionResult<ApiResponse<string>>> CalculateSalary([FromBody] UpdateSalaryDto updateSalaryDto)
        {
            var response = await _salaryManagmentService.CalculateSalary(updateSalaryDto);
            if (!response.Success)
            {
                return BadRequest(response); // استخدم ApiResponse مباشرة
            }

            return Ok(response); // استخدم ApiResponse مباشرة
        }
        [HttpPost("GetAll/{Id}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SalaryDto>>>> GetAllInsuranceEmployees(FilterModel filterModel,int Id, int? ContractId)
        {
            var response = await _salaryManagmentService.GetAllEmployeeSalaryAsync(filterModel, Id, ContractId);
          
            return Ok(response);
        }
        [HttpPost("GetAllDsaByEmployeeId/{id}")]
        public async Task<ActionResult<ApiResponse<List<GetAllDsaDto>>>> GetAllDsaByEmployeeId(int id,int? ContractId, FilterModel filterModel)
        {
            var response = await _salaryManagmentService.GetAllDsaByEmployeddId(id, ContractId, filterModel);
           
            return Ok(response);
        }

        // ✅ Add DSA to Salary
        [HttpPost("AddDsa")]
        public async Task<ActionResult<ApiResponse<string>>> AddDsa([FromBody] AddDsaDto addDsaDto)
        {
            var response = await _salaryManagmentService.AddDsa(addDsaDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        // Get By Id GetEmployee Salary ById 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResponse<GetByIdSalaryDto>>> GetEmployeeSalaryByIdAsync(int Id)
        {
            var response = await _salaryManagmentService.GetEmployeeSalaryByIdAsync(Id);
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("Employee with the provided ID not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response); // استخدم ApiResponse مباشرة
        }
    }
}
