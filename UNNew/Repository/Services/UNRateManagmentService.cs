using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.UNRateDto;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class UNRateManagmentService: IUNRateManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UNRateManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<ApiResponse<string>> CreateUnRateAsync(CreateUNRateDto newUnRate)
        {
            try
            {
                UnRate unRate = new UnRate()
                {

                    MonthNum = newUnRate.MonthNum,
                    YearNum = newUnRate.YearNum,
                    UnRate1 = newUnRate.ExchangeRate,
                    CreatedAt=DateTime.UtcNow,
                    ClientId= newUnRate.ClientId

                };
                // إضافة سجل جديد إلى قاعدة البيانات
                await _context.UnRates.AddAsync(unRate);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Created successfully", unRate.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while creating the record.", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<UNRateDto>> GetUnRateByIdAsync(int id)
        {
            try
            {
                // البحث عن السجل باستخدام الـ id
                var unRate = await _context.UnRates
                    .Where(x => x.Id == id)
                    .Include(x=>x.client)
                    .Select(x => new UNRateDto
                    {
                        Id = x.Id,
                        YearNum = x.YearNum,
                        MonthNum = x.MonthNum,
                        ExchangeRate = x.UnRate1 ,
                        CreatedAt=x.CreatedAt,// تعيين قيمة الـ UnRate1 إلى ExchangeRate في الـDto
                        ClientName=x.client.ClientName
                    })
                    .FirstOrDefaultAsync();

                // إذا لم يتم العثور على السجل، نرجع رسالة خطأ
                if (unRate == null)
                {
                    return new ApiResponse<UNRateDto>(
                        "Record not found",
                        null,
                        new List<string> { "No record found with the provided ID." }
                    );
                }

                // إذا تم العثور على السجل، نرجع الـ UNRateDto
                return new ApiResponse<UNRateDto>("Success", unRate, null);
            }
            catch (Exception ex)
            {
                // في حال حدوث استثناء، نرجع رسالة خطأ
                return new ApiResponse<UNRateDto>(
                    "An error occurred while retrieving the exchange rate record.",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }
        public async Task<ApiResponse<List<UNRateDto>>> GetAllUnRatesAsync()
        {
            try
            {
                // جلب وترتيب السجلات من UnRates
                var unRates = await _context.UnRates
                    .Include(x => x.client)
                    .OrderByDescending(x => x.YearNum)
                    .ThenByDescending(x => x.MonthNum)
                    .ThenByDescending(x => x.CreatedAt)
                    .Select(x => new UNRateDto
                    {
                        Id = x.Id,
                        YearNum = x.YearNum,
                        MonthNum = x.MonthNum,
                        ExchangeRate = x.UnRate1,
                        CreatedAt = x.CreatedAt,
                        ClientName = x.client.ClientName
                    })
                    .ToListAsync();

                if (unRates == null || !unRates.Any())
                {
                    return new ApiResponse<List<UNRateDto>>(
                        "No records found",
                        null,
                        new List<string> { "There are no exchange rate records available." }
                    );
                }

                return new ApiResponse<List<UNRateDto>>("Success", unRates, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<UNRateDto>>(
                    "An error occurred while retrieving exchange rate records.",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<string>> UpdateUnRateAsync(UNRateDto updatedUnRate)
        {
            try
            {
                var existingUnRate = await _context.UnRates.FirstOrDefaultAsync(x => x.Id == updatedUnRate.Id);
                if (existingUnRate == null)
                {
                    return new ApiResponse<string>("Record not found.", null, new List<string> { "The record with the given ID does not exist." });
                }

                // تحديث الحقول
                existingUnRate.YearNum = updatedUnRate.YearNum;
                existingUnRate.MonthNum = updatedUnRate.MonthNum;
                existingUnRate.UnRate1 = updatedUnRate.ExchangeRate;

                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Record updated successfully.", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while updating the record.", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<string>> DeleteUnRateAsync(int id)
        {
            try
            {
                var unRateToDelete = await _context.UnRates.FirstOrDefaultAsync(x => x.Id == id);
                if (unRateToDelete == null)
                {
                    return new ApiResponse<string>("Record not found.", null, new List<string> { "The record with the given ID does not exist." });
                }

                _context.UnRates.Remove(unRateToDelete);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Record deleted successfully.", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while deleting the record.", null, new List<string> { ex.Message });
            }
        }

    }
}
