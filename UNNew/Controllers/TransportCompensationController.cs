using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.TransportCompensationDto;
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
    public class TransportCompensationController : ControllerBase
    {
        private readonly ITransportCompensationService  _transportCompensationService;
        private readonly IMapper _mapper;

        public TransportCompensationController(ITransportCompensationService transportCompensationService, IMapper mapper)
        {
            _transportCompensationService = transportCompensationService;
            _mapper = mapper;
        }

        // Create UN Rate
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateUNRate([FromBody] AddTransportCompensationDto addTransportCompensationDto)
        {
         
            var Response = await _transportCompensationService.CreateTransportCompensationAsync(addTransportCompensationDto);
            return Ok(Response);

        }

       

        // Get All UN Rates
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<GetAllTransportCompensationDto>>>> GetAllUNRates()
        {
            var response = await _transportCompensationService.GetAllUnTransportCompensationAsync();
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
