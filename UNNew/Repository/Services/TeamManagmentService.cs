using UNNew.DTOS.TeamDtos;
using UNNew.Models;
using UNNew.Response;
using Microsoft.EntityFrameworkCore;
using UNNew.Repository.Interfaces;

namespace UNNew.Repository.Services
{
    public class TeamManagmentService : ITeamManagmentService
    {
        private readonly UNDbContext _context;

        public TeamManagmentService(UNDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> CreateTeamAsync(CreateTeamDto createTeamDto)
        {
            try
            {
                // التحقق من أن اسم الفريق غير فارغ
                if (string.IsNullOrWhiteSpace(createTeamDto.TeamName))
                {
                    return new ApiResponse<string>("Team name is required", null);
                }

                // التحقق من أن اسم الفريق فريد داخل نفس العميل
                bool teamExists = await _context.Teams
                    .AnyAsync(t => t.TeamName == createTeamDto.TeamName && t.ClientId == createTeamDto.ClientId);

                if (teamExists)
                {
                    return new ApiResponse<string>(
                        "A team with this name already exists in this client",
                        null,
                        new List<string> { "Duplicate team name" });
                }

                // إنشاء كائن Team جديد
                var team = new Team
                {
                    TeamName = createTeamDto.TeamName,
                    ClientId = createTeamDto.ClientId
                    // TeamId يُفترض أنه auto-increment
                };

                _context.Teams.Add(team);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Team created successfully", team.TeamId.ToString());
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    "Error creating team",
                    null,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> UpdateTeamAsync(UpdateTeamDto updateTeamDto)
        {
            try
            {
                var team = await _context.Teams.FindAsync(updateTeamDto.TeamId);
                if (team == null)
                {
                    return new ApiResponse<string>(
                        "Team not found",
                        null,
                        new List<string> { "Team with the given ID not found." }
                    );
                }

                if (string.IsNullOrEmpty(updateTeamDto.TeamName))
                {
                    return new ApiResponse<string>(
                        "Team name is required",
                        null,
                        new List<string> { "Team name cannot be empty." }
                    );
                }

                bool teamNameExists = await _context.Teams
                    .AnyAsync(t => t.TeamName == updateTeamDto.TeamName && t.TeamId != updateTeamDto.TeamId);
                if (teamNameExists)
                {
                    return new ApiResponse<string>(
                        "A team with this name already exists",
                        null,
                        new List<string> { "Another team with this name already exists." }
                    );
                }

                team.TeamName = updateTeamDto.TeamName;
                team.ClientId = updateTeamDto.ClientId;

                _context.Teams.Update(team);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Team updated successfully", team.TeamId.ToString());
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    "Error updating team",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<TeamDto>> GetTeamByIdAsync(int teamId)
        {
            try
            {
                var team = await _context.Teams
                    .Where(t => t.TeamId == teamId)
                    .Select(t => new TeamDto
                    {
                        TeamId = t.TeamId,
                        TeamName = t.TeamName,
                        ClientId = t.ClientId,
                        ClientName = t.Client != null ? t.Client.ClientName : string.Empty // أو null
                    })
                    .FirstOrDefaultAsync();

                if (team == null)
                {
                    return new ApiResponse<TeamDto>(
                        "Team not found",
                        null,
                        new List<string> { "No team found with the provided ID." }
                    );
                }

                return new ApiResponse<TeamDto>("Team fetched successfully", team);
            }
            catch (Exception ex)
            {
                return new ApiResponse<TeamDto>(
                    "Error fetching team",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<TeamDto>>> GetAllTeamsAsync()
        {
            try
            {
                var teams = await _context.Teams
                    .Include(x => x.Client)
                    .Select(t => new TeamDto
                    {
                        TeamId = t.TeamId,
                        TeamName = t.TeamName,
                        ClientId = t.ClientId,
                        ClientName = t.Client != null ? t.Client.ClientName : string.Empty // أو null
                    })
                    .ToListAsync();

                if (teams == null || !teams.Any())
                {
                    return new ApiResponse<IEnumerable<TeamDto>>(
                        "No teams found",
                        null,
                        new List<string> { "No teams are available in the system." }
                    );
                }

                return new ApiResponse<IEnumerable<TeamDto>>("Teams fetched successfully", teams);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<TeamDto>>(
                    "Error fetching teams",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<string>> DeleteTeamAsync(int teamId)
        {
            try
            {
                var contract = _context.EmployeeCoos.FirstOrDefault(x => x.TeamId == teamId);
                if (contract != null)
                {
                    return new ApiResponse<string>(
                        "Cannot delete this team because it is associated with an active contract.",
                        null,
                        new List<string> { "Team is associated with an active contract." }
                    );
                }

                var team = await _context.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);
                if (team == null)
                {
                    return new ApiResponse<string>(
                        "Team not found",
                        null,
                        new List<string> { "No team found with the provided ID." }
                    );
                }

                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>(
                    "Team deleted successfully",
                    null,
                    new List<string>() // Errors فاضية → Success = true
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    "Error deleting team",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }



    }
}
