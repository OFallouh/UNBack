using UNNew.DTOS.UnLaptopCompensationDto;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IUnLaptopCompensationService
    {
        Task<ApiResponse<string>> CreateLaptopCompensationAsync(AddLaptopCompensationDto addLaptopCompensationDto);
        Task<ApiResponse<List<GetAllLaptopCompensationDto>>> GetAllUnLaptopCompensationAsync();
    }
}
