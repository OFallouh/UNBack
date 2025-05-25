using AutoMapper;
using UNNew.DTOS.RoleDtos;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class RoleManagmentService : IRoleManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public RoleManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public ApiResponse<string> CreateRole(string RoleName)
        {
            try
            {
                var role = new Role
                {
                    Name = RoleName
                };
                _context.Roles.Add(role);
                _context.SaveChanges();
                return new ApiResponse<string>("Role created successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error", null, new List<string> { ex.Message });
            }
        }

        public ApiResponse<string> UpdateRole(UpdateRoleDto updateRole)
        {
            try
            {
                var role = _context.Roles.FirstOrDefault(r => r.Id == updateRole.RoleId);
                if (role == null)
                {
                    return new ApiResponse<string>("Role not found", null, new List<string> { "Role not found" });
                }

                role.Name = updateRole.RoleName;
                _context.SaveChanges();
                return new ApiResponse<string>("Role updated successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error", null, new List<string> { ex.Message });
            }
        }

        public ApiResponse<string> DeleteRole(int id)
        {
            try
            {
                var role = _context.Roles.FirstOrDefault(r => r.Id == id);
                if (role == null)
                {
                    return new ApiResponse<string>("Role not found", null, new List<string> { "Role not found" });
                }

                _context.Roles.Remove(role);
                _context.SaveChanges();
                return new ApiResponse<string>("Role deleted successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error", null, new List<string> { ex.Message });
            }
        }
        public ApiResponse<List<RoleDto>> GetAllRoles()
        {
            try
            {
                var roles = _context.Roles
                .Select(role => new RoleDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                })
                .ToList();

                return new ApiResponse<List<RoleDto>>("Roles retrieved successfully", roles, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<RoleDto>>("Error", null, new List<string> { ex.Message });
            }
        }
    }
}
