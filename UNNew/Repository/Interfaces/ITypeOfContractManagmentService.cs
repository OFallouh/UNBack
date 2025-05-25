using UNNew.DTOS.TypeOfContractDtos;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface ITypeOfContractManagmentService
    {
        Task<ApiResponse<string>> CreateTypeOfContractAsync(CreateTypeOfContractDto createTypeOfContractDto);
        Task<ApiResponse<string>> UpdateTypeOfContractAsync(UpdateTypeOfContractDto updateTypeOfContractDto);
        Task<ApiResponse<TypeOfContractDto>> GetTypeOfContractByIdAsync(int typeOfContractId);
        Task<ApiResponse<IEnumerable<TypeOfContractDto>>> GetAllTypeOfContractsAsync();
        Task<ApiResponse<string>> DeleteTypeOfContractAsync(int typeOfContractId);
    }
}
