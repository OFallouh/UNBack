using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.BankDtos;
using UNNew.DTOS.LaptopDto;
using UNNew.Helpers;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountCompanyManagmentController : ControllerBase
    {
        private readonly IAccountCompanyManagmentService _accountCompanyManagmentService;
        private readonly IMapper _mapper;

        public AccountCompanyManagmentController(IAccountCompanyManagmentService accountCompanyManagmentService, IMapper mapper)
        {
            _accountCompanyManagmentService = accountCompanyManagmentService;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<AccountCompanyDto>>> GetAll()
        {
            var data = await _accountCompanyManagmentService.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountCompanyDto>> GetById(int id)
        {
            var result = await _accountCompanyManagmentService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
     

        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> Create([FromBody] AddAccountCompanyDto dto)
        {
            var response = await _accountCompanyManagmentService.CreateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);

        }


        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id,[FromBody] AddAccountCompanyDto dto)
        {
            var response = await _accountCompanyManagmentService.UpdateAsync(id,dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _accountCompanyManagmentService.DeleteAsync(id);

            // افترض أن `response` هو من نوع ApiResponse<bool> ويحتوي على خاصية `Data` أو `Success`
            if (!response.Data) // أو إذا كان `Success` بدلاً من `Data`
            {
                return NotFound();
            }
            return Ok(new { message = "Deleted" });
        }

    }
}
