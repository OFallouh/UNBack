using UNNew.DTOS.TeamDtos;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface ITeamManagmentService
    {
        Task<ApiResponse<string>> CreateTeamAsync(CreateTeamDto createTeamDto);
        Task<ApiResponse<string>> UpdateTeamAsync(UpdateTeamDto updateTeamDto);
        Task<ApiResponse<TeamDto>> GetTeamByIdAsync(int teamId);
        Task<ApiResponse<IEnumerable<TeamDto>>> GetAllTeamsAsync();
        Task<ApiResponse<string>> DeleteTeamAsync(int teamId);
    }
}
