using UNNew.DTOS.TransportCompensationDto;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface ITransportCompensationService
    {
        Task<ApiResponse<string>> CreateTransportCompensationAsync(AddTransportCompensationDto addTransportCompensationDto);
        Task<ApiResponse<List<GetAllTransportCompensationDto>>> GetAllUnTransportCompensationAsync();
    }
}
