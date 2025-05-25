using UNNew.DTOS.LaptopDto;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface ILaptopRentManagmentService
    {
        Task<ApiResponse<string>> CreateLaptopRentAsync(CreateLaptopRentDto createLaptopRentDto);
        Task<ApiResponse<IEnumerable<LaptopRentDto>>> GetAllLaptopRentsAsync();
        Task<ApiResponse<string>> CreateLaptopTypeAsync(CreateLaptopTypeDto createLaptopTypeDto);
        Task<ApiResponse<string>> UpdateLaptopTypeAsync(UpdateLaptopTypeDto updateLaptopTypeDto);
        Task<ApiResponse<IEnumerable<GetLaptopTypeDto>>> GetAllLaptopTypesAsync();
        Task<ApiResponse<string>> DeleteLaptopTypeAsync(int id);
        Task<ApiResponse<string>> UpdateLaptopRentAsync(UpdateLaptopRentDto updateLaptopRentDto);
        Task<ApiResponse<string>> DeleteLaptopRentAsync(int id);
        Task<ApiResponse<GetAllGetLaptopTypeDto>> GetLaptopTypeByIdAsync(int id);
    }
}
