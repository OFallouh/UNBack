using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.CurrencyDtos;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class CurrencyManagmentService : ICurrencyManagmentService
    {

        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CurrencyManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }


        // Create Currency
        public async Task<ApiResponse<string>> CreateCurrencyAsync(CreateCurrencyDto createCurrencyDto)
        {
            try
            {
                // التحقق من وجود Type
                bool typeExists = await _context.Currencies.AnyAsync(c => c.Type == createCurrencyDto.Type);
                if (typeExists)
                {
                    return new ApiResponse<string>("Currency type already exists", null, new List<string> { "A currency with the same type already exists." });
                }

                // الحصول على آخر Id وإضافة واحد
                int lastCurrencyId = await _context.Currencies.MaxAsync(c => (int?)c.Id) ?? 0;
                int newCurrencyId = lastCurrencyId + 1;

                var currency = new Currency
                {
                    Id = newCurrencyId, // تعيين Id الجديد
                    Type = createCurrencyDto.Type
                };

                _context.Currencies.Add(currency);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Currency created successfully", currency.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while creating the currency", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> UpdateCurrencyAsync(UpdateCurrencyDto updateCurrencyDto)
        {
            try
            {
                var currency = await _context.Currencies.FindAsync(updateCurrencyDto.Id);
                if (currency == null)
                {
                    return new ApiResponse<string>("Currency not found", null, new List<string> { "Currency with the provided ID not found" });
                }

                // التحقق من وجود Type
                bool typeExists = await _context.Currencies.AnyAsync(c => c.Type == updateCurrencyDto.Type && c.Id != updateCurrencyDto.Id);
                if (typeExists)
                {
                    return new ApiResponse<string>("Currency type already exists", null, new List<string> { "A currency with the same type already exists." });
                }

                currency.Type = updateCurrencyDto.Type;

                _context.Currencies.Update(currency);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Currency updated successfully", currency.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while updating the currency", null, new List<string> { ex.Message });
            }
        }

        // Get Currency by ID
        public async Task<ApiResponse<CurrencyDto>> GetCurrencyByIdAsync(int id)
        {
            var currency = await _context.Currencies
                .Where(c => c.Id == id)
                .Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Type = c.Type
                })
                .FirstOrDefaultAsync();

            if (currency == null)
            {
                return new ApiResponse<CurrencyDto>("Currency not found", null);
            }

            return new ApiResponse<CurrencyDto>("Currency fetched successfully", currency);
        }

        // Get All Currencies
        public async Task<ApiResponse<IEnumerable<CurrencyDto>>> GetAllCurrenciesAsync()
        {
            var currencies = await _context.Currencies
                .Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Type = c.Type
                })
                .ToListAsync();

            return new ApiResponse<IEnumerable<CurrencyDto>>("Currencies fetched successfully", currencies);
        }

        // Delete Currency
        public async Task<ApiResponse<string>> DeleteCurrencyAsync(int id)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var contrat = _context.EmployeeCoos.FirstOrDefault(x => x.Coo.CurrencyId == id && x.EndCont >= today && x.IsCancelled == false);
            if(contrat!=null)
                return new ApiResponse<string>(
                        "Cannot delete this Currency because it is associated with an active contract.",
                        null,
                        new List<string> { "Client is associated with an active contract." }
                    );

            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return new ApiResponse<string>("Currency not found", null);
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();

            return new ApiResponse<string>("Currency deleted successfully", null);
        }
    }
}

