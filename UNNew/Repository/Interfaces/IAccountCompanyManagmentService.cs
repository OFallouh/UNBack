using UNNew.DTOS.BankDtos;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IAccountCompanyManagmentService
    {
        Task<ApiResponse<List<AccountCompanyDto>>> GetAllAsync();
        Task<ApiResponse<AccountCompanyDto?>> GetByIdAsync(int id);
        Task<ApiResponse<int>> CreateAsync(AddAccountCompanyDto dto);
        Task<ApiResponse<bool>> UpdateAsync(int id, AddAccountCompanyDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
