using UNNew.DTOS.InvoiceDto;
using UNNew.Filters;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IInvoiceManagmentService
    {
        Task<ApiResponse<List<string>>> GetAllPoNumber();
        Task<ApiResponse<List<GetAllInvoiceDto>>> GetAllPoNumber(GetAllDetailsInvoice getAllDetailsInvoice);
        Task<ApiResponse<string>> CancelInvoiceAsync(int id);
        Task<ApiResponse<IEnumerable<InvoiceDto>>> GetAllInvoicesAsync(FilterModel filterRequest);
        Task<ApiResponse<InvoiceDto>> GetInvoiceByIdAsync(int id);
        Task<ApiResponse<string>> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto);
        Task<ApiResponse<string>> UpdateInvoiceAsync(UpdateInvoiceDto updateInvoiceDto);
        Task<ApiResponse<string>> GetLastInvoiceNumber(int ClientName);
    }
}
