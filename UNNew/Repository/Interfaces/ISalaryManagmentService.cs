using UNNew.DTOS.DsaDto;
using UNNew.DTOS.LaptopDto;
using UNNew.DTOS.SalaryDtos;
using UNNew.Filters;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface ISalaryManagmentService
    {
        Task<ApiResponse<GetByIdSalaryDto>> GetEmployeeSalaryByIdAsync(int id);
        Task<ApiResponse<string>> CalculateSalary(UpdateSalaryDto updateSalaryDto);
        Task<ApiResponse<string>> AddDsa(AddDsaDto addDsaDto);
        Task<ApiResponse<List<SalaryDto>>> GetAllEmployeeSalaryAsync(FilterModel filterModel, int Id, int? ContractId);


        Task<ApiResponse<List<GetAllDsaDto>>> GetAllDsaByEmployeddId(int id, int? ContractId, FilterModel filterModel);
    }
}
