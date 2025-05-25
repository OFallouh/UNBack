using UNNew.DTOS.PurchaseOrder;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IPurchaseOrderManagmentService
    {
        Task<ApiResponse<string>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto createPurchaseOrderDto);
        Task<ApiResponse<string>> UpdatePurchaseOrderAsync(UpdatePurchaseOrderDto updatePurchaseOrderDto);
        Task<ApiResponse<AllPurchaseOrderDto>> GetPurchaseOrderByIdAsync(int orderId);
        Task<ApiResponse<IEnumerable<AllPurchaseOrderDto>>> GetAllPurchaseOrdersAsync();
        Task<ApiResponse<string>> DeletePurchaseOrderAsync(int orderId);
    }
}
