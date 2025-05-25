using UNNew.DTOS.UNRateDto;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IUNRateManagmentService
    {
        Task<ApiResponse<string>> CreateUnRateAsync(CreateUNRateDto newUnRate);
        Task<ApiResponse<UNRateDto>> GetUnRateByIdAsync(int id);
        Task<ApiResponse<List<UNRateDto>>> GetAllUnRatesAsync();
        Task<ApiResponse<string>> UpdateUnRateAsync(UNRateDto updatedUnRate);
        Task<ApiResponse<string>> DeleteUnRateAsync(int id);
    }
}
