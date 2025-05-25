using Microsoft.AspNetCore.Mvc;
using UNNew.DTOS.BankDtos;
using UNNew.Response;

namespace UNNew.Repository.Interfaces
{
    public interface IBankManagmentService
    {
        Task<ApiResponse<List<BankDto>>> GetAllBanksAsync();

        // Get a bank by its ID
        Task<ApiResponse<BankDto>> GetBankByIdAsync(int id);

        // Update an existing bank
        Task<ApiResponse<string>> UpdateBankAsync(int id, string BanksName, IFormFile? files);
        // Delete a bank by its ID
        Task<ApiResponse<string>> DeleteBankAsync(int id);

        Task<ApiResponse<string>> CreateBankAsync(string BanksName, IFormFile? files);
    }
}
