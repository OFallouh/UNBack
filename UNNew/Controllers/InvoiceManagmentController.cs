using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.InvoiceDto;
using UNNew.Filters;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceManagmentController : ControllerBase
    {
        private readonly IInvoiceManagmentService _invoiceManagmentService;

        public InvoiceManagmentController(IInvoiceManagmentService invoiceManagmentService)
        {
            _invoiceManagmentService = invoiceManagmentService;
        }
        //// Get all PO Numbers
        //[HttpGet("GetAllPoNumbersAsync")]
        //public async Task<ActionResult<ApiResponse<List<string>>>> GetAllPoNumbersAsync()
        //{
        //    var response = await _invoiceManagmentService.GetAllPoNumber();
        //    if (response.Data == null || !response.Data.Any())
        //    {
        //        return NotFound(new ApiResponse<List<string>>("No PO numbers found", null, new List<string> { "No data available" }));
        //    }

        //    return Ok(response);
        //}

        // Get PO details by number, year, and month
        [HttpPost("GetAllDetails")]
        public async Task<ActionResult<ApiResponse<List<GetAllInvoiceDto>>>> GetPoDetailsAsync([FromBody] GetAllDetailsInvoice getAllDetailsInvoice)
        {
            var response = await _invoiceManagmentService.GetAllPoNumber(getAllDetailsInvoice);
            if (response.Data == null || !response.Data.Any())
            {
                return NotFound(new ApiResponse<List<GetAllInvoiceDto>>("No data found for this PO number", null, new List<string> { "Check PO number or date inputs." }));
            }

            return Ok(response);
        }
        [HttpPost("GetAllInvoices")]
        public async Task<ActionResult<ApiResponse<IEnumerable<InvoiceDto>>>> GetAllInvoicesAsync([FromBody] FilterModel filterRequest)
        {
            var response = await _invoiceManagmentService.GetAllInvoicesAsync(filterRequest);

            if (!response.Data.Any())
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        // Get invoice by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetInvoiceById(int id)
        {
            var response = await _invoiceManagmentService.GetInvoiceByIdAsync(id);

            if (response.Data == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        [HttpGet("GetLastInvoiceNumber/{ClientId}")]
        public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetLastInvoiceNumber(int ClientId)
        {
            var response = await _invoiceManagmentService.GetLastInvoiceNumber(ClientId);

            if (response.Data == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        // Create new invoice
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateInvoiceAsync([FromBody] CreateInvoiceDto createInvoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>("Invalid data", null, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var response = await _invoiceManagmentService.CreateInvoiceAsync(createInvoiceDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetInvoiceById), new { id = response.Data }, response);
        }

        // Update existing invoice
        [HttpPut]
        public async Task<ActionResult<ApiResponse<string>>> UpdateInvoiceAsync([FromBody] UpdateInvoiceDto updateInvoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>("Invalid data", null, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var response = await _invoiceManagmentService.UpdateInvoiceAsync(updateInvoiceDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

       
     
    }
}
