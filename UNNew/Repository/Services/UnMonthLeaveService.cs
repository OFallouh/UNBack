using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.UnMonthLeaveDto;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class UnMonthLeaveService: IUnMonthLeaveService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UnMonthLeaveService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponse<string>> CreateUnMonthLeaveAsync(AddUnMonthLeaveDto dto)
        {
            try
            {
                UnMonthLeave unMonthLeave = new UnMonthLeave
                {
                    YearNum = dto.YearNum,
                    MonthNum = dto.MonthNum,
                    UnMonthLeave1 = dto.UnMonthLeave1,
                    ClientId = dto.ClientId,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.UnMonthLeave.AddAsync(unMonthLeave);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Created successfully", unMonthLeave.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while creating the record.", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<List<GetAllUnMonthLeaveDto>>> GetAllUnMonthLeaveAsync()
        {
            try
            {
                var unMonthLeaves = await _context.UnMonthLeave
                    .Include(x => x.Client)
                    .OrderByDescending(x => x.YearNum)
                    .ThenByDescending(x => x.MonthNum)
                    .ThenByDescending(x => x.CreatedAt)
                    .Select(x => new GetAllUnMonthLeaveDto
                    {
                        Id = x.Id,
                        YearNum = x.YearNum,
                        MonthNum = x.MonthNum,
                        UnMonthLeave1 = x.UnMonthLeave1,
                        CreatedAt = x.CreatedAt,
                        ClientName = x.Client.ClientName
                    })
                    .ToListAsync();

                if (unMonthLeaves == null || !unMonthLeaves.Any())
                {
                    return new ApiResponse<List<GetAllUnMonthLeaveDto>>(
                        "No records found",
                        null,
                        new List<string> { "There are no UnMonthLeave records available." }
                    );
                }

                return new ApiResponse<List<GetAllUnMonthLeaveDto>>("Success", unMonthLeaves, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetAllUnMonthLeaveDto>>(
                    "An error occurred while retrieving UnMonthLeave records.",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }


    }
}
