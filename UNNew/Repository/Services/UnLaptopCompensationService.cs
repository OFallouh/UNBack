using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.UnLaptopCompensationDto;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{

    public class UnLaptopCompensationService: IUnLaptopCompensationService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UnLaptopCompensationService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<ApiResponse<string>> CreateLaptopCompensationAsync(AddLaptopCompensationDto addLaptopCompensationDto)
        {
            try
            {
                UnMobileCompensation unLaptopCompensation = new UnMobileCompensation()
                {
                    MonthNum = addLaptopCompensationDto.MonthNum,
                    YearNum = addLaptopCompensationDto.YearNum,
                    UnMobileCompensation1 = addLaptopCompensationDto.MobileCompensation,
                    CreatedAt = DateTime.UtcNow,
                    ClientId = addLaptopCompensationDto.ClientId
                };

                // إضافة سجل جديد إلى قاعدة البيانات
                await _context.UnMobileCompensation.AddAsync(unLaptopCompensation);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Created successfully", unLaptopCompensation.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while creating the record.", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<List<GetAllLaptopCompensationDto>>> GetAllUnLaptopCompensationAsync()
        {
            try
            {
                // جلب جميع السجلات من UnLaptopCompensations وترتيبها
                var unLaptopCompensations = await _context.UnMobileCompensation
                    .Include(x => x.client)
                    .OrderByDescending(x => x.YearNum)
                    .ThenByDescending(x => x.MonthNum)
                    .ThenByDescending(x => x.CreatedAt)
                    .Select(x => new GetAllLaptopCompensationDto
                    {
                        Id = x.Id,
                        YearNum = x.YearNum,
                        MonthNum = x.MonthNum,
                        MobileCompensation = x.UnMobileCompensation1,
                        CreatedAt = x.CreatedAt,
                        ClientName = x.client.ClientName
                    })
                    .ToListAsync();

                if (unLaptopCompensations == null || !unLaptopCompensations.Any())
                {
                    return new ApiResponse<List<GetAllLaptopCompensationDto>>(
                        "No records found",
                        null,
                        new List<string> { "There are no exchange LaptopCompensation records available." }
                    );
                }

                return new ApiResponse<List<GetAllLaptopCompensationDto>>("Success", unLaptopCompensations, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetAllLaptopCompensationDto>>(
                    "An error occurred while retrieving exchange LaptopCompensation records.",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }


    }
}
