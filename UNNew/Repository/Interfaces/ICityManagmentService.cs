using UNNew.DTOS.CityDtos;
using UNNew.Models;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface ICityManagmentService
    {
        Task<ApiResponse<IEnumerable<CityDto>>> GetAllCitiesAsync();
        Task<ApiResponse<string>> CreateCityAsync(CreateCityDto createCityDto);
        Task<ApiResponse<string>> UpdateCityAsync(UpdateCityDto updateCityDto);
        Task<ApiResponse<string>> DeleteCityAsync(int id);
        Task<ApiResponse<CityDto>> GetCityByIdAsync(int id);
    }
}
