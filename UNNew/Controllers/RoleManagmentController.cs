using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.RoleDtos;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;
namespace UNNew.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleManagmentController : ControllerBase
    {
        private IRoleManagmentService _roleManagmentService;
        private readonly IConfiguration _configuration;

        public RoleManagmentController(IRoleManagmentService roleManagmentService, IConfiguration configuration)
        {
            _roleManagmentService = roleManagmentService;
            _configuration = configuration;
        }
        //[HttpPost("CreateRole")]
        //public IActionResult CreateRole(string RoleName)
        //{
        //    var response = _roleManagmentService.CreateRole(RoleName);
        //    if (response?.Errors?.Any() == true)
        //    {
        //        return BadRequest(response); // يجب أن يكون متاح الآن
        //    }

        //    return Ok(response);
        //}

        //[HttpPost("UpdateRole")]
        //public IActionResult UpdateRole([FromBody] UpdateRoleDto updateRole)
        //{
        //    var response = _roleManagmentService.UpdateRole(updateRole);
        //    if (response?.Errors?.Any() == true)
        //    {
        //        return BadRequest(response); // يجب أن يكون متاح الآن
        //    }

        //    return Ok(response);
        //}

        //[HttpDelete("DeleteRole")]
        //public IActionResult DeleteRole(int id)
        //{
        //    var response = _roleManagmentService.DeleteRole(id);
        //    if (response?.Errors?.Any() == true)
        //    {
        //        return BadRequest(response); // يجب أن يكون متاح الآن
        //    }

        //    return Ok(response);
        //}
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var response = _roleManagmentService.GetAllRoles();

            if (response?.Errors?.Any() == true)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
    }
}
