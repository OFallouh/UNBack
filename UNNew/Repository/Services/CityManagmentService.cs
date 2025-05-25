using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.CityDtos;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class CityManagmentService : ICityManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CityManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        // Get All Cities API
        public async Task<ApiResponse<IEnumerable<CityDto>>> GetAllCitiesAsync()
        {
            try
            {
                var cities = await _context.Cities.ToListAsync();
                if (cities == null || !cities.Any())
                {
                    return new ApiResponse<IEnumerable<CityDto>>("No cities found", null, new List<string> { "No data available" });
                }

                var cityDtos = cities.Select(city => new CityDto
                {
                    CityId = city.CityId,
                    NameAr = city.NameAr,
                    NameEn = city.NameEn
                }).ToList();

                return new ApiResponse<IEnumerable<CityDto>>("Cities fetched successfully", cityDtos, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CityDto>>("Error occurred while fetching cities", null, new List<string> { ex.Message });
            }
        }

        // Create City API
        public async Task<ApiResponse<string>> CreateCityAsync(CreateCityDto createCityDto)
        {
            try
            {
                // التحقق من صحة المدخلات
                if (createCityDto == null || string.IsNullOrEmpty(createCityDto.NameAr) || string.IsNullOrEmpty(createCityDto.NameEn))
                {
                    return new ApiResponse<string>("Invalid input", null, new List<string> { "City names (Arabic and English) are required." });
                }

                // التحقق من وجود اسم المدينة (بالعربية)
                bool cityArExists = await _context.Cities.AnyAsync(c => c.NameAr == createCityDto.NameAr);
                if (cityArExists)
                {
                    return new ApiResponse<string>("City name (Arabic) already exists", null, new List<string> { "A city with the same Arabic name already exists." });
                }

                // التحقق من وجود اسم المدينة (بالإنجليزية)
                bool cityEnExists = await _context.Cities.AnyAsync(c => c.NameEn == createCityDto.NameEn);
                if (cityEnExists)
                {
                    return new ApiResponse<string>("City name (English) already exists", null, new List<string> { "A city with the same English name already exists." });
                }

                // الحصول على آخر CityId وإضافة واحد
                int lastCityId = await _context.Cities.MaxAsync(c => (int?)c.CityId) ?? 0;
                int newCityId = lastCityId + 1;

                var city = new City
                {
                    NameAr = createCityDto.NameAr,
                    NameEn = createCityDto.NameEn
                };

                await _context.Cities.AddAsync(city);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    return new ApiResponse<string>("Error creating city", null, new List<string> { "Unable to save city to database." });
                }

                return new ApiResponse<string>("City created successfully", city.CityId.ToString(), null);
            }
            catch (DbUpdateException ex)
            {
                // معالجة أخطاء قاعدة البيانات
                return new ApiResponse<string>("Database error occurred while creating the city", null, new List<string> { ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                // معالجة الأخطاء العامة
                return new ApiResponse<string>("Error occurred while creating the city", null, new List<string> { ex.Message });
            }
        }

        // Update City API
        public async Task<ApiResponse<string>> UpdateCityAsync(UpdateCityDto updateCityDto)
        {
            try
            {
                var city = await _context.Cities.FindAsync(updateCityDto.CityId);
                if (city == null)
                {
                    return new ApiResponse<string>("City not found", null, new List<string> { "City with the provided ID not found" });
                }

                // التحقق من وجود اسم المدينة (بالعربية)
                bool cityArExists = await _context.Cities.AnyAsync(c => c.NameAr == updateCityDto.NameAr && c.CityId != updateCityDto.CityId);
                if (cityArExists)
                {
                    return new ApiResponse<string>("City name (Arabic) already exists", null, new List<string> { "A city with the same Arabic name already exists." });
                }

                // التحقق من وجود اسم المدينة (بالإنجليزية)
                bool cityEnExists = await _context.Cities.AnyAsync(c => c.NameEn == updateCityDto.NameEn && c.CityId != updateCityDto.CityId);
                if (cityEnExists)
                {
                    return new ApiResponse<string>("City name (English) already exists", null, new List<string> { "A city with the same English name already exists." });
                }

                city.NameAr = updateCityDto.NameAr;
                city.NameEn = updateCityDto.NameEn;

                _context.Cities.Update(city);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    return new ApiResponse<string>("Error updating city", null, new List<string> { "Unable to update city" });
                }

                return new ApiResponse<string>("City updated successfully", city.CityId.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while updating the city", null, new List<string> { ex.Message });
            }
        }

        // Delete City API
        public async Task<ApiResponse<string>> DeleteCityAsync(int id)
        {
            try
            {
                var city = await _context.Cities.FindAsync(id);
                if (city == null)
                {
                    return new ApiResponse<string>(
                        "City not found",
                        null,
                        new List<string> { "City with the provided ID not found" }
                    );
                }

                // التحقق من وجود موظفين مرتبطين
                bool hasRelatedCity = await _context.UnEmps.AnyAsync(emp => emp.CityId == id);
                if (hasRelatedCity)
                {
                    return new ApiResponse<string>(
                        "Cannot delete city because it is associated with employees",
                        null,
                        new List<string> { "City is associated with existing employees" }
                    );
                }

                _context.Cities.Remove(city);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    return new ApiResponse<string>(
                        "Error deleting city",
                        null,
                        new List<string> { "Unable to delete city" }
                    );
                }

                return new ApiResponse<string>(
                    "City deleted successfully",
                    null,
                    new List<string>() // Errors فارغة تعني Success = true
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    "Error occurred while deleting the city",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }

        // Get City By Id API
        public async Task<ApiResponse<CityDto>> GetCityByIdAsync(int id)
        {
            try
            {
                var city = await _context.Cities.FindAsync(id); // العثور على المدينة باستخدام المعرف
                if (city == null)
                {
                    return new ApiResponse<CityDto>("City not found", null, new List<string> { "City with the provided ID not found" });
                }

                // تحويل المدينة إلى DTO
                var cityDto = new CityDto
                {
                    CityId = city.CityId,
                    NameAr = city.NameAr,
                    NameEn = city.NameEn
                };

                return new ApiResponse<CityDto>("City fetched successfully", cityDto, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CityDto>("Error occurred while fetching city", null, new List<string> { ex.Message });
            }
        }

    }
}
