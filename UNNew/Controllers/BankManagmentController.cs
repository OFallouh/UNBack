using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.BankDtos;
using UNNew.Response;
using UNNew.Repository.Interfaces;
using UNNew.Repository.Services;
using UNNew.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace UNNew.Controllers
{
    //[Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class BankManagementController : ControllerBase
    {
        private readonly IBankManagmentService _bankManagementService;
        private readonly IMapper _mapper;

        public BankManagementController(IBankManagmentService bankManagementService, IMapper mapper)
        {
            _bankManagementService = bankManagementService;
            _mapper = mapper;
        }

        // Get All Banks
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<BankDto>>>> GetAllBanks()
        {
            try
            {
                var response = await _bankManagementService.GetAllBanksAsync();

                if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
                {
                    if (response.Message == "No banks found" || response.Message == "No data available")
                    {
                        return NotFound(response); // إرجاع NotFound مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
                    }
                    else
                    {
                        return BadRequest(response); // إرجاع BadRequest للأخطاء الأخرى
                    }
                }

                return Ok(response); // إرجاع Ok في حالة النجاح
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<BankDto>>("Error occurred while retrieving banks", null, new List<string> { ex.Message }));
            }
        }

        // Get Bank by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BankDto>>> GetBankById(int id)
        {
            try
            {
                var response = await _bankManagementService.GetBankByIdAsync(id);

                if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
                {
                    if (response.Message == "Bank not found" || response.Message == "Invalid bank ID")
                    {
                        return NotFound(response); // إرجاع NotFound مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
                    }
                    else
                    {
                        return BadRequest(response); // إرجاع BadRequest للأخطاء الأخرى
                    }
                }

                return Ok(response); // إرجاع Ok في حالة النجاح
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<BankDto>("Error occurred while retrieving the bank", null, new List<string> { ex.Message }));
            }
        }

        // Create Bank
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AddBankDto>>> CreateBank( string BanksName, IFormFile? files)
        {
            try
            {
               
                if (string.IsNullOrEmpty(BanksName))
                {
                    return BadRequest(new ApiResponse<AddBankDto>("Invalid input", null, new List<string> { "Bank name is required." }));
                }
                // يمكن إضافة المزيد من التحققات هنا حسب الحاجة

                var response = await _bankManagementService.CreateBankAsync(BanksName, files);

                if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
                {
                    return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
                }

                return CreatedAtAction(nameof(GetBankById), new { id = response.Data }, response); // Return the created bank
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AddBankDto>("Error occurred while creating the bank", null, new List<string> { ex.Message }));
            }
        }

        // Update Bank
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UpdateBankDto>>> UpdateBank(int id, [FromQuery] string BanksName, IFormFile? files)
        {
            try
            {
                if (string.IsNullOrEmpty(BanksName))
                {
                    return BadRequest(new ApiResponse<UpdateBankDto>("Invalid input", null, new List<string> { "Bank name is required." }));
                }

                // يمكن إضافة المزيد من التحققات هنا حسب الحاجة

                var response = await _bankManagementService.UpdateBankAsync(id, BanksName, files);

                if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
                {
                    return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
                }

                return Ok(response); // إرجاع Ok في حالة النجاح
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UpdateBankDto>("Error occurred while updating the bank", null, new List<string> { ex.Message }));
            }
        }


        // Delete Bank
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteBank(int id)
        {
            try
            {
                var response = await _bankManagementService.DeleteBankAsync(id);

                if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
                {
                    return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
                }

                return Ok(response); // إرجاع Ok في حالة النجاح
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>("Error occurred while deleting the bank", null, new List<string> { ex.Message }));
            }
        }

    }
}
