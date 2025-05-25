using UNNew.DTOS.InsuranceDtos;
using UNNew.Filters;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IInsuranceManagmentService
    {
        Task<ApiResponse<IEnumerable<InsuranceEmployeeDto>>> GetAllInsuranceEmployeeAsync(FilterModel filterModel);
        Task<ApiResponse<InsuranceEmployeeDto>> GetInsuranceEmployeeByIdAsync(int EmployeeId);
        Task<ApiResponse<string>> UpdateInsuranceEmployeeAsync(UpdateInsuranceDto updateInsuranceDto);

    }
}
