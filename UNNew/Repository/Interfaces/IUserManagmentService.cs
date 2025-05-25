using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.UserDTO;
using UNNew.DTOS.UserDtos;
using UNNew.Filters;
using UNNew.Models;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IUserManagmentService
    {
        ApiResponse<string> LoginAccount(LoginDto loginDto);
        ApiResponse<string> CreateUser(AddUserDto createUserDto);
        ApiResponse<string> DeleteUser(int userId);
        ApiResponse<string> UpdateUser(UpdateUserDto updateUser);
        ApiResponse<List<UserDto>> GetAllUsers(FilterModel request);
        ApiResponse<UserDto> GetUserById(int userId);
        ApiResponse<string> ChangePassword(ChangePasswordDto changePasswordDto);
        ApiResponse<string> ResetPasswordForAdmin(ResetPasswordDto resetPasswordDto);
        ApiResponse<string> Logout();
    }
}
