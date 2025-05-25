using UNNew.DTOS.ContractDtos;
using UNNew.DTOS.PoNumberDto;
using UNNew.Filters;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IContractManagmentService
    {
        Task<ApiResponse<IEnumerable<ContractDto>>> GetAllContractsAsync(FilterModel filterModel,int? Id);
        Task<ApiResponse<GetByIdContractDto>> GetContractByIdAsync(int id);
        Task<ApiResponse<string>> CreateContract(AddContractDto addContractDto);
        Task<ApiResponse<string>> UpdateContractAsync(int Id,UpdateContractDto updateContractDto);
        Task<ApiResponse<string>> DeleteContractAsync(int id);
        Task<ApiResponse<string>> CancelContractAsync(int id, BeforCancelContractDto beforCancelContractDto);
        Task<ApiResponse<GetByIdContractDto>> GetContractByEmployeeIdAsync(int Id);
        Task<ApiResponse<string>> UpdateEndContractBeforCancel(int Id,BeforCancelContractDto beforCancelContractDto);
        Task<ApiResponse<List<GetAllPoNumberDto>>> GetAllPoNumber();
    }
}
