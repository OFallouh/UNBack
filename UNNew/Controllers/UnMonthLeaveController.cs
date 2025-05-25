using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.UnLaptopCompensationDto;
using UNNew.DTOS.UnMonthLeaveDto;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Controllers
{    //[Authorize]
    //[ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class UnMonthLeaveController : ControllerBase
    {
        private readonly IUnMonthLeaveService  _unMonthLeaveService;
        private readonly IMapper _mapper;

        public UnMonthLeaveController(IUnMonthLeaveService unMonthLeaveService, IMapper mapper)
        {
            _unMonthLeaveService = unMonthLeaveService;
            _mapper = mapper;
        }

        // Create UN Rate
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateUnMonthLeave([FromBody] AddUnMonthLeaveDto addUnMonthLeaveDto)
        {

            var Response = await _unMonthLeaveService.CreateUnMonthLeaveAsync(addUnMonthLeaveDto);
            return Ok(Response);

        }


        // Get All UN Rates
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<GetAllUnMonthLeaveDto>>>> GetAllUNRates()
        {
            var response = await _unMonthLeaveService.GetAllUnMonthLeaveAsync();
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
    }
}
