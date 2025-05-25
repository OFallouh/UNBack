using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.ClientDtos;
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
    public class ClientManagmentController : ControllerBase
    {
        private readonly IClientManagmentService _clientService;

        public ClientManagmentController(IClientManagmentService clientService)
        {
            _clientService = clientService;
        }

        // Get All Clients
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ClientDto>>>> GetAllClients()
        {
            var response = await _clientService.GetAllClientsAsync();
            return Ok(response);
        }

        // Get Client By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ClientDto>>> GetClientById(int id)
        {
            var response = await _clientService.GetClientByIdAsync(id);
            if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
            {
                return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
            }


            return Ok(response);
        }

        // Create Client
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateClient([FromBody] CreateClientDto createClientDto)
        {
            var response = await _clientService.CreateClientAsync(createClientDto);
            if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
            {
                return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
            }

            return CreatedAtAction(nameof(GetClientById), new { id = response.Data }, response);
        }

        // Update Client
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateClient(int id, [FromBody] UpdateClientDto updateClientDto)
        {
            if (id != updateClientDto.ClientId)
            {
                return BadRequest("Client ID mismatch");
            }

            var response = await _clientService.UpdateClientAsync(updateClientDto);
            if (response.Errors != null && response.Errors.Any()) // التحقق من وجود أخطاء في الاستجابة
            {
                return BadRequest(response); // إرجاع BadRequest مع الاستجابة الكاملة (بما في ذلك رسالة الخطأ)
            }

            return Ok(response); // إرجاع Ok في حالة النجاح

        }
    
        // Delete Bank
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteClient(int id)
        {
            try
            {
                var response = await _clientService.DeleteClientAsync(id);

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



