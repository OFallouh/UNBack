using UNNew.DTOS.UnMonthLeaveDto;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IUnMonthLeaveService
    {
        Task<ApiResponse<string>> CreateUnMonthLeaveAsync(AddUnMonthLeaveDto dto);
        Task<ApiResponse<List<GetAllUnMonthLeaveDto>>> GetAllUnMonthLeaveAsync();
    }
}
