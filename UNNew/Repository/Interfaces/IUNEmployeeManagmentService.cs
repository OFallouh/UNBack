using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.FileDto;
using UNNew.DTOS.UNEmployeeDtos;
using UNNew.Filters;
using UNNew.Models;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IUNEmployeeManagmentService
    {
        Task<ApiResponse<string>> CreateUNEmployeeAsync(CreateUNEmployeeDto createUNEmployeeDto);
        Task<ApiResponse<string>> UpdateUNEmployeeAsync(UpdateUNEmployeeDto updatedEmployee,int Id);
        Task<ApiResponse<bool>> DeleteUNEmployeeAsync(int UNEmployeeId);
        Task<ApiResponse<bool>> DisableUNEmployeeAsync(int id);
        Task<ApiResponse<DisplyUNEmployeeDto>> GetByIdEmployeeAsync(int RefNo);
        Task<ApiResponse<IEnumerable<FileResponseDto>>> UploadFilesAsync(int refNo, List<IFormFile> files);
        Task<ApiResponse<IEnumerable<FileResponseDto>>> GetEmployeeFilesAsync(int refNo);
        Task<ApiResponse<(byte[] content, string contentType, string fileName)>> DownloadFileAsync(int refNo, string fileName);
        Task<ApiResponse<bool>> DeleteFileAsync(int refNo, string fileName);
        Task<ApiResponse<bool>> DeleteAllFilesAsync(int refNo);
        Task<ApiResponse<List<UNEmployeeDto>>> GetAllEmployeesAsync(FilterModel filterRequest, string? PoNumber);

    }
}
