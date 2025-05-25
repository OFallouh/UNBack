using UNNew.DTOS.CurrencyDtos;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface ICurrencyManagmentService
    {
        Task<ApiResponse<string>> CreateCurrencyAsync(CreateCurrencyDto createCurrencyDto);
        Task<ApiResponse<string>> UpdateCurrencyAsync(UpdateCurrencyDto updateCurrencyDto);
        Task<ApiResponse<CurrencyDto>> GetCurrencyByIdAsync(int id);
        Task<ApiResponse<IEnumerable<CurrencyDto>>> GetAllCurrenciesAsync();
        Task<ApiResponse<string>> DeleteCurrencyAsync(int id);
    }
}
