using UNNew.DTOS.CooDtos;
using UNNew.DTOS.FileDto;
using UNNew.DTOS.UNEmployeeDtos;
using UNNew.Filters;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface ICooManagmentService
    {
        Task<ApiResponse<string>> CreateCooAsync(CreateCooDto createCooDto);
        Task<ApiResponse<string>> UpdateCooAsync(UpdateCooDto updateCooDto, int Id);
        Task<ApiResponse<GetByIdDto>> GetCooByIdAsync(int cooId);
        Task<ApiResponse<string>> DeleteCooAsync(int cooId);
        Task<ApiResponse<IEnumerable<CooDto>>> GetAllCoosAsync(FilterModel filterModel);
        Task<ApiResponse<IEnumerable<GetAllCooWithoutPaginationDto>>> GetAllCooWithoutPagination();
        Task<ApiResponse<IEnumerable<FileResponseDto>>> GetCooFilesAsync(int cooId);
        Task<ApiResponse<(byte[] content, string contentType, string fileName)>> DownloadCooFileAsync(int cooId, string fileName);
        Task<ApiResponse<bool>> DeleteCooFileAsync(int cooId, string fileName);
        Task<ApiResponse<bool>> DeleteAllCooFilesAsync(int cooId);
        Task<ApiResponse<IEnumerable<FileResponseDto>>> UploadFilesForCooAsync(int cooId, List<IFormFile> files);
    }
}
