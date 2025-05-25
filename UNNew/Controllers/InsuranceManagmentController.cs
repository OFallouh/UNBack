using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.InsuranceDtos;
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
    public class InsuranceManagmentController : ControllerBase
    {
        private readonly IInsuranceManagmentService _insuranceService;
        private readonly IMapper _mapper;

        public InsuranceManagmentController(IInsuranceManagmentService insuranceService, IMapper mapper)
        {
            _insuranceService = insuranceService;
            _mapper = mapper;
        }

        // Get All Insurance Employees
        [HttpPost("GetAll")]
        public async Task<ActionResult<ApiResponse<IEnumerable<InsuranceEmployeeDto>>>> GetAllInsuranceEmployees(FilterModel filterModel)
        {
            var response = await _insuranceService.GetAllInsuranceEmployeeAsync(filterModel);
            if (response.Data == null || !response.Data.Any())
            {
                return NotFound(new ApiResponse<IEnumerable<InsuranceEmployeeDto>>(
                    "No insurance employees found",
                    null,
                    new List<string> { "No data available" }));
            }

            return Ok(response);
        }

        // Get Insurance Employee By Id
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResponse<InsuranceEmployeeDto>>> GetInsuranceEmployeeById(int Id)
        {
            var response = await _insuranceService.GetInsuranceEmployeeByIdAsync(Id);
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("Employee is not Active"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }

        // Update Insurance Employee
        [HttpPut("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateInsuranceEmployee([FromBody] UpdateInsuranceDto updateInsuranceDto)
        {
            var response = await _insuranceService.UpdateInsuranceEmployeeAsync(updateInsuranceDto);
            if (!response.Success)
            {
                if (response.Errors != null && response.Errors.Contains("Employee is not Active"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }

        
    }
}

