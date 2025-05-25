using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.ClientDtos;
using UNNew.DTOS.TeamDtos;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
using UNNew.Response;


namespace UNNew.Controllers
{
  //  [Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamManagmentController : ControllerBase
    {
        private readonly ITeamManagmentService _teamService;

        public TeamManagmentController(ITeamManagmentService teamService)
        {
            _teamService = teamService;
        }

        // Create Team
       
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateTeam([FromBody] CreateTeamDto createTeamDto)
        {
            var response = await _teamService.CreateTeamAsync(createTeamDto);
            if (response.Data == null)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetTeamById), new { id = response.Data }, response);
        }

        // Update Team
        [HttpPut("{Id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateTeamAsync([FromBody] UpdateTeamDto updateTeamDto)
        {
            var response = await _teamService.UpdateTeamAsync(updateTeamDto);
            if (response?.Errors?.Any() == true)
            {
                return BadRequest(response);  // إذا كانت هناك أخطاء
            }

            return Ok(response);
        }

        // Get Team By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TeamDto>>> GetTeamById(int id)
        {
            var response = await _teamService.GetTeamByIdAsync(id);
            if (response.Data == null)
            {
                return NotFound(new ApiResponse<TeamDto>("Team not found", null, new List<string> { "No team found with the provided ID" }));
            }

            return Ok(response);
        }

        // Get All Teams
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TeamDto>>>> GetAllTeamsAsync()
        {
            var response = await _teamService.GetAllTeamsAsync();
            if (response?.Errors?.Any() == true)
            {
                return BadRequest(response);  // إذا كانت هناك أخطاء
            }

            return Ok(response);
        }

        // Delete Team
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteTeamAsync(int id)
        {
            var response = await _teamService.DeleteTeamAsync(id);
            if (response?.Errors?.Any() == true)
            {
                return BadRequest(response);  // إذا كانت هناك أخطاء
            }

            return Ok(response);
        }
    }
}



