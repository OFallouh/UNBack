using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.BankDtos;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class AccountCompanyManagmentService: IAccountCompanyManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccountCompanyManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<ApiResponse<List<AccountCompanyDto>>> GetAllAsync()
        {
            try
            {
                var accountCompanies = await _context.AccountCompany
                    .Include(x => x.Bank)
                    .Select(ac => new AccountCompanyDto
                    {
                        AccountCompanyId = ac.AccountCompanyId,
                        BanksId = ac.BanksId,
                        BankName=ac.Bank.BanksName,
                        Region = ac.Region,
                        Branch = ac.Branch,
                        AccountNumber = ac.AccountNumber,
                        BankLogoUrl = ac.Bank.BankLogoUrl
                    })
                    .ToListAsync();

                if (accountCompanies.Count == 0)
                {
                    return new ApiResponse<List<AccountCompanyDto>>("No data found.", null, new List<string> { "No account companies available." });
                }

                return new ApiResponse<List<AccountCompanyDto>>("Data retrieved successfully.", accountCompanies, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AccountCompanyDto>>("Error retrieving data.", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AccountCompanyDto?>> GetByIdAsync(int id)
        {
            try
            {
                var ac = await _context.AccountCompany
                    .Include(x => x.Bank)
                    .FirstOrDefaultAsync(x => x.AccountCompanyId == id);

                if (ac == null)
                {
                    return new ApiResponse<AccountCompanyDto?>("Account company not found.", null, new List<string> { "No account company found with this ID." });
                }

                var accountCompanyDto = new AccountCompanyDto
                {
                    AccountCompanyId = ac.AccountCompanyId,
                    BanksId = ac.BanksId,
                    BankName=ac.Bank.BanksName,
                    Region = ac.Region,
                    Branch = ac.Branch,
                    AccountNumber = ac.AccountNumber,
                    BankLogoUrl = ac.Bank?.BankLogoUrl
                };

                return new ApiResponse<AccountCompanyDto?>("Data retrieved successfully.", accountCompanyDto, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AccountCompanyDto?>("Error retrieving data.", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(AddAccountCompanyDto dto)
        {
            try
            {
                // Optional: Validate bank ID before creation
                bool bankExists = await _context.Banks.AnyAsync(b => b.BanksId == dto.BanksId);
                if (!bankExists)
                {
                    return new ApiResponse<int>("Bank not found.", -1, new List<string> { "The specified bank does not exist." });
                }

                // Validate if the account already exists
                bool accountExists = await _context.AccountCompany
                    .AnyAsync(ac => ac.BanksId == dto.BanksId && ac.AccountNumber == dto.AccountNumber && ac.Branch == dto.Branch && ac.Region==dto.Region);

                if (accountExists)
                {
                    return new ApiResponse<int>("Account already exists.", -1, new List<string> { "An account with this bank and account number already exists." });
                }

                var entity = new AccountCompany
                {
                    BanksId = dto.BanksId,
                    Region = dto.Region,
                    Branch = dto.Branch,
                    AccountNumber = dto.AccountNumber
                };

                _context.AccountCompany.Add(entity);
                await _context.SaveChangesAsync();

                return new ApiResponse<int>("Account company created successfully.", entity.AccountCompanyId, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>("Error creating account company.", -1, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<bool>> UpdateAsync(int id, AddAccountCompanyDto dto)
        {
            try
            {
                var ac = await _context.AccountCompany.FirstOrDefaultAsync(x => x.AccountCompanyId == id);
                if (ac == null)
                {
                    return new ApiResponse<bool>("Account company not found.", false, new List<string> { "No account company found with this ID." });
                }

                // Check if the bank exists
                bool bankExists = await _context.Banks.AnyAsync(b => b.BanksId == dto.BanksId);
                if (!bankExists)
                {
                    return new ApiResponse<bool>("Bank not found.", false, new List<string> { "The specified bank does not exist." });
                }

                // Validate if the account already exists (check for duplicates excluding the current account)
                bool accountExists = await _context.AccountCompany
                    .AnyAsync(ac => ac.BanksId == dto.BanksId && ac.AccountNumber == dto.AccountNumber && ac.Branch == dto.Branch && ac.Region == dto.Region && ac.AccountCompanyId != id);

                if (accountExists)
                {
                    return new ApiResponse<bool>("Account already exists.", false, new List<string> { "An account with this bank and account number already exists." });
                }

                // Update account company fields
                ac.BanksId = dto.BanksId;
                ac.Region = dto.Region;
                ac.Branch = dto.Branch;
                ac.AccountNumber = dto.AccountNumber;

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>("Account company updated successfully.", true, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>("Error updating account company.", false, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var ac = await _context.AccountCompany.FirstOrDefaultAsync(x => x.AccountCompanyId == id);
                if (ac == null)
                {
                    return new ApiResponse<bool>("Account company not found.", false, new List<string> { "No account company found with this ID." });
                }

                _context.AccountCompany.Remove(ac);
                await _context.SaveChangesAsync();
                return new ApiResponse<bool>("Account company deleted successfully.", true, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>("Error deleting account company.", false, new List<string> { ex.Message });
            }
        }
    }
}
