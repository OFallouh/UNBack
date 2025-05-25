using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.TransportCompensationDto;
using UNNew.DTOS.UnLaptopCompensationDto;
using UNNew.DTOS.UNRateDto;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Controllers
{
    //[Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class UnMobileCompensationController : ControllerBase
    {
        private readonly IUnLaptopCompensationService _unLaptopCompensationService;
        private readonly IMapper _mapper;

        public UnMobileCompensationController(IUnLaptopCompensationService unLaptopCompensationService, IMapper mapper)
        {
            _unLaptopCompensationService = unLaptopCompensationService;
            _mapper = mapper;
        }

        // Create UN Rate
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateUNRate([FromBody] AddLaptopCompensationDto addLaptopCompensationDto)
        {

            var Response = await _unLaptopCompensationService.CreateLaptopCompensationAsync(addLaptopCompensationDto);
            return Ok(Response);

        }

        // Get All UN Rates
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<GetAllLaptopCompensationDto>>>> GetAllUNRates()
        {
            var response = await _unLaptopCompensationService.GetAllUnLaptopCompensationAsync();
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
