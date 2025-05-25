using UNNew.Models;
using UNNew.Response;
using AutoMapper;
using UNNew.DTOS.PurchaseOrder;
using Microsoft.EntityFrameworkCore;
using UNNew.Repository.Interfaces;

namespace UNNew.Repository.Services
{
    public class PurchaseOrderManagmentService : IPurchaseOrderManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PurchaseOrderManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        // Create Purchase Order
        public async Task<ApiResponse<string>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto createPurchaseOrderDto)
        {
            var purchaseOrder = new PurchaseOrder
            {
                PoNo = createPurchaseOrderDto.PoNo,
                PoAmount = createPurchaseOrderDto.PoAmount,
                Cooid = createPurchaseOrderDto.Cooid
            };

            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            return new ApiResponse<string>("Purchase Order created successfully", purchaseOrder.OrderId.ToString());
        }

        // Update Purchase Order
        public async Task<ApiResponse<string>> UpdatePurchaseOrderAsync(UpdatePurchaseOrderDto updatePurchaseOrderDto)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(updatePurchaseOrderDto.OrderId);
            if (purchaseOrder == null)
            {
                return new ApiResponse<string>("Purchase Order not found", null);
            }

            purchaseOrder.PoNo = updatePurchaseOrderDto.PoNo;
            purchaseOrder.PoAmount = updatePurchaseOrderDto.PoAmount;
            purchaseOrder.Cooid = updatePurchaseOrderDto.Cooid;

            _context.PurchaseOrders.Update(purchaseOrder);
            await _context.SaveChangesAsync();

            return new ApiResponse<string>("Purchase Order updated successfully", purchaseOrder.OrderId.ToString());
        }

        // Get Purchase Order by ID
        public async Task<ApiResponse<AllPurchaseOrderDto>> GetPurchaseOrderByIdAsync(int orderId)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .Where(po => po.OrderId == orderId)
                .Select(po => new AllPurchaseOrderDto
                {
                    OrderId = po.OrderId,
                    PoNo = po.PoNo,
                    PoAmount = po.PoAmount,
                    Cooid = po.Cooid
                })
                .FirstOrDefaultAsync();

            if (purchaseOrder == null)
            {
                return new ApiResponse<AllPurchaseOrderDto>("Purchase Order not found", null);
            }

            return new ApiResponse<AllPurchaseOrderDto>("Purchase Order fetched successfully", purchaseOrder);
        }

        // Get All Purchase Orders
        public async Task<ApiResponse<IEnumerable<AllPurchaseOrderDto>>> GetAllPurchaseOrdersAsync()
        {
            var purchaseOrders = await _context.PurchaseOrders
                .Select(po => new AllPurchaseOrderDto
                {
                    OrderId = po.OrderId,
                    PoNo = po.PoNo,
                    PoAmount = po.PoAmount,
                    Cooid = po.Cooid
                })
                .ToListAsync();

            return new ApiResponse<IEnumerable<AllPurchaseOrderDto>>("Purchase Orders fetched successfully", purchaseOrders);
        }

        // Delete Purchase Order
        public async Task<ApiResponse<string>> DeletePurchaseOrderAsync(int orderId)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(orderId);
            if (purchaseOrder == null)
            {
                return new ApiResponse<string>("Purchase Order not found", null);
            }

            _context.PurchaseOrders.Remove(purchaseOrder);
            await _context.SaveChangesAsync();

            return new ApiResponse<string>("Purchase Order deleted successfully", null);
        }
    }
}

