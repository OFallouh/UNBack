using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.UserDTO;
using UNNew.DTOS.UserDtos;
using UNNew.Filters;
using UNNew.Helpers;
using UNNew.Repository.Interfaces;

namespace UNNew.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagmentService _userManagmentService;
        private readonly IConfiguration _configuration;

        public UserManagementController(IUserManagmentService userManagmentService, IConfiguration configuration)
        {
            _userManagmentService = userManagmentService;
            _configuration = configuration;
        }
        //[Authorize]
        // تسجيل الدخول
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("Invalid login data.");
            }

            var response = _userManagmentService.LoginAccount(loginDto);

            if (response?.Errors?.Any() == true)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
        //[Authorize]
        // إنشاء مستخدم جديد
        [HttpPost]
        public IActionResult CreateUser([FromBody] AddUserDto loginDto)
        {
            var response = _userManagmentService.CreateUser(loginDto);

            if (response?.Errors?.Any() == true)
            {
                return BadRequest(response);  // إذا كانت هناك أخطاء
            }

            return CreatedAtAction(nameof(GetUserById), new { id = response.Data }, response);
        }
       // [Authorize]
        // تحديث بيانات المستخدم
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var response = _userManagmentService.UpdateUser(updateUserDto);

            if (response?.Errors?.Any() == true)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        [Authorize]
        // حذف مستخدم
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var response = _userManagmentService.DeleteUser(id);

            if (response?.Errors?.Any() == true)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        //[Authorize]
        // جلب جميع المستخدمين
        [HttpPost("GetAllUsers")]
        public IActionResult GetAllUsers(FilterModel request)
        {
            var response = _userManagmentService.GetAllUsers( request);

            if (response?.Errors?.Any() == true)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        [Authorize]
        // جلب مستخدم واحد حسب الـ ID
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var response = _userManagmentService.GetUserById(id);

            if (response == null)
            {
                return NotFound("User not found.");
            }

            return Ok(response);
        }
        [Authorize]
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var response = _userManagmentService.ChangePassword(changePasswordDto);

            if (response.Errors != null && response.Errors.Count > 0)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost("ResetPasswordForAdmin")]
        public IActionResult ResetPasswordForAdmin([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var response = _userManagmentService.ResetPasswordForAdmin(resetPasswordDto);

            if (response.Errors != null && response.Errors.Count > 0)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var response = _userManagmentService.Logout();

            if (response.Errors != null && response.Errors.Count > 0)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
