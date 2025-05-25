
using UNNew.DTOS.RoleDtos;
using UNNew.DTOS.UserDTO;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IRoleManagmentService
    {
        ApiResponse<string> CreateRole(string RoleName);
        ApiResponse<string> UpdateRole(UpdateRoleDto updateRole);
        ApiResponse<string> DeleteRole(int id);
        ApiResponse<List<RoleDto>> GetAllRoles();
       
    }
}
