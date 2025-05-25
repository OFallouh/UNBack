using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.CityDtos;
using UNNew.DTOS.TransportCompensationDto;
using UNNew.DTOS.UNRateDto;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class TransportCompensationService : ITransportCompensationService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TransportCompensationService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<ApiResponse<string>> CreateTransportCompensationAsync(AddTransportCompensationDto addTransportCompensationDto)
        {
            try
            {
                UnTransportCompensation unRate = new UnTransportCompensation()
                {

                    MonthNum = addTransportCompensationDto.MonthNum,
                    YearNum = addTransportCompensationDto.YearNum,
                    UnTransportCompensation1 = addTransportCompensationDto.TransportCompensation,
                    CreatedAt = DateTime.UtcNow,
                    ClientId = addTransportCompensationDto.ClientId

                };
                // إضافة سجل جديد إلى قاعدة البيانات
                await _context.UnTransportCompensation.AddAsync(unRate);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Created successfully", unRate.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while creating the record.", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<List<GetAllTransportCompensationDto>>> GetAllUnTransportCompensationAsync()
        {
            try
            {
                // جلب جميع السجلات من UnRates
                var unRates = await _context.UnTransportCompensation
                    .Include(x => x.client)
                    .Select(x => new GetAllTransportCompensationDto
                    {
                        Id = x.Id,
                        YearNum = x.YearNum,
                        MonthNum = x.MonthNum,
                         TransportCompensation= x.UnTransportCompensation1,
                        CreatedAt = x.CreatedAt,
                        ClientName = x.client.ClientName


                    })
                    .ToListAsync();

                // التحقق إذا كانت القائمة فارغة
                if (unRates == null || !unRates.Any())
                {
                    return new ApiResponse<List<GetAllTransportCompensationDto>>(
                        "No records found",
                        null,
                        new List<string> { "There are no exchange TransportCompensation records available." }
                    );
                }

                // إرجاع السجلات
                return new ApiResponse<List<GetAllTransportCompensationDto>>("Success", unRates, null);
            }
            catch (Exception ex)
            {
                // في حال حدوث استثناء، نرجع رسالة خطأ
                return new ApiResponse<List<GetAllTransportCompensationDto>>(
                    "An error occurred while retrieving exchange TransportCompensation records.",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
