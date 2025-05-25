using UNNew.DTOS.ClientDtos;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IClientManagmentService
    {
        Task<ApiResponse<IEnumerable<ClientDto>>> GetAllClientsAsync();
        Task<ApiResponse<string>> CreateClientAsync(CreateClientDto createClientDto);
        Task<ApiResponse<ClientDto>> GetClientByIdAsync(int clientId);
        Task<ApiResponse<string>> UpdateClientAsync(UpdateClientDto updateClientDtoint);
        Task<ApiResponse<string>> DeleteClientAsync(int id);

    }
}
