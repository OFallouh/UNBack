//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using UNNew.DTOS.PurchaseOrder;
//using UNNew.Helpers;
//using UNNew.Repository.Interfaces;
//using UNNew.Response;

//namespace UNNew.Controllers
//{
//    [Authorize]
//    [ServiceFilter(typeof(LogFilterAttribute))]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PurchaseOrderManagmentController : ControllerBase
//    {
//        private readonly IPurchaseOrderManagmentService _purchaseOrderService;

//        public PurchaseOrderManagmentController(IPurchaseOrderManagmentService purchaseOrderService)
//        {
//            _purchaseOrderService = purchaseOrderService;
//        }

//        // Create a new Purchase Order
//        [HttpPost]
//        public async Task<ActionResult<ApiResponse<string>>> CreatePurchaseOrder([FromBody] CreatePurchaseOrderDto createPurchaseOrderDto)
//        {
//            var response = await _purchaseOrderService.CreatePurchaseOrderAsync(createPurchaseOrderDto);
//            if (response.Data != null)
//            {
//                return CreatedAtAction(nameof(GetPurchaseOrderById), new { id = response.Data }, response);
//            }

//            return BadRequest(response);
//        }

//        // Update an existing Purchase Order
//        [HttpPut]
//        public async Task<ActionResult<ApiResponse<string>>> UpdatePurchaseOrder([FromBody] UpdatePurchaseOrderDto updatePurchaseOrderDto)
//        {
//            var response = await _purchaseOrderService.UpdatePurchaseOrderAsync(updatePurchaseOrderDto);
//            if (response.Data != null)
//            {
//                return Ok(response);
//            }

//            return BadRequest(response);
//        }

//        // Get Purchase Order by ID
//        [HttpGet("{id}")]
//        public async Task<ActionResult<ApiResponse<AllPurchaseOrderDto>>> GetPurchaseOrderById(int id)
//        {
//            var response = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
//            if (response.Data != null)
//            {
//                return Ok(response);
//            }

//            return NotFound(response);
//        }

//        // Get All Purchase Orders
//        [HttpGet]
//        public async Task<ActionResult<ApiResponse<IEnumerable<AllPurchaseOrderDto>>>> GetAllPurchaseOrders()
//        {
//            var response = await _purchaseOrderService.GetAllPurchaseOrdersAsync();
//            if (response.Data != null && response.Data.Any())
//            {
//                return Ok(response);
//            }

//            return NoContent();
//        }

//        // Delete a Purchase Order by ID
//        [HttpDelete("{id}")]
//        public async Task<ActionResult<ApiResponse<string>>> DeletePurchaseOrder(int id)
//        {
//            var response = await _purchaseOrderService.DeletePurchaseOrderAsync(id);
//            if (response.Data == null)
//            {
//                return NotFound(response);
//            }

//            return Ok(response);
//        }
//    }
//}



