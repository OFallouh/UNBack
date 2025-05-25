using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.LaptopDto;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class LaptopRentManagmentService : ILaptopRentManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public LaptopRentManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<ApiResponse<string>> CreateLaptopRentAsync(CreateLaptopRentDto createLaptopRentDto)
        {
            try
            {
                // التحقق من وجود بيانات صحيحة
                if (createLaptopRentDto.Price <= 0)
                {
                    return new ApiResponse<string>("Invalid price", null, new List<string> { "Invalid price" });
                }

                    // التحقق من أن LaptopType موجود
                bool laptopTypeExists = await _context.LaptopTypes.AnyAsync(t => t.Id == createLaptopRentDto.LaptopType);
                if (!laptopTypeExists)
                {
                    return new ApiResponse<string>("Invalid LaptopType", null, new List<string> { "Invalid LaptopType" });
                }

                // إنشاء كائن LaptopRent جديد
                var laptopRent = new LaptopRent
                {
                    Year = createLaptopRentDto.Year,
                    Month = createLaptopRentDto.Month,
                    Price = createLaptopRentDto.Price,
                    LaptopType = createLaptopRentDto.LaptopType,
                    CreatedAt=DateTime.UtcNow,
                };

                // إضافة البيانات إلى قاعدة البيانات
                _context.LaptopRents.Add(laptopRent);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("LaptopRent created successfully", laptopRent.Id.ToString());
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error creating LaptopRent", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<IEnumerable<LaptopRentDto>>> GetAllLaptopRentsAsync()
        {
            try
            {
                var laptopRents = await _context.LaptopRents
                    .Include(r => r.LaptopTypeNavigation)
                    .OrderByDescending(r => r.Year)
                    .ThenByDescending(r => r.Month)
                    .ThenByDescending(r => r.CreatedAt)
                    .Select(r => new LaptopRentDto
                    {
                        Id = r.Id,
                        Year = r.Year,
                        Month = r.Month,
                        Price = r.Price,
                        LaptopTypeName = r.LaptopTypeNavigation != null ? r.LaptopTypeNavigation.Name : string.Empty,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();

                if (laptopRents == null || !laptopRents.Any())
                {
                    return new ApiResponse<IEnumerable<LaptopRentDto>>("No LaptopRents found", null, new List<string> { "No LaptopRents found" });
                }

                return new ApiResponse<IEnumerable<LaptopRentDto>>("LaptopRents fetched successfully", laptopRents);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<LaptopRentDto>>("Error fetching LaptopRents", null, new List<string> { "Error fetching LaptopRents", ex.Message });
            }
        }


        public async Task<ApiResponse<string>> CreateLaptopTypeAsync(CreateLaptopTypeDto createLaptopTypeDto)
        {
            try
            {
                if (string.IsNullOrEmpty(createLaptopTypeDto.Name))
                {
                    return new ApiResponse<string>("Laptop type name is required", null, new List<string> { "Laptop type name is required" });
                }

                bool laptopTypeExists = await _context.LaptopTypes.AnyAsync(t => t.Name == createLaptopTypeDto.Name);
                if (laptopTypeExists)
                {
                    return new ApiResponse<string>("A laptop type with this name already exists", null, new List<string> { "A laptop type with this name already exists" });
                }

                var laptopType = new LaptopType
                {
                    Name = createLaptopTypeDto?.Name,
                    Activite = createLaptopTypeDto?.Activite
                };

                _context.LaptopTypes.Add(laptopType);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Laptop type created successfully", laptopType.Id.ToString());
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error creating laptop type", null, new List<string> { "Error creating laptop type", ex.Message });
            }
        }

        public async Task<ApiResponse<string>> UpdateLaptopTypeAsync(UpdateLaptopTypeDto updateLaptopTypeDto)
        {
            try
            {
                var laptopType = await _context.LaptopTypes.FindAsync(updateLaptopTypeDto.Id);
                if (laptopType == null)
                {
                    return new ApiResponse<string>("Laptop type not found", null, new List<string> { "Laptop type not found" });
                }

                if (string.IsNullOrEmpty(updateLaptopTypeDto.Name))
                {
                    return new ApiResponse<string>("Laptop type name is required", null, new List<string> { "Laptop type name is required" });
                }

                bool laptopTypeNameExists = await _context.LaptopTypes
                    .AnyAsync(t => t.Name == updateLaptopTypeDto.Name && t.Id != updateLaptopTypeDto.Id);
                if (laptopTypeNameExists)
                {
                    return new ApiResponse<string>("A laptop type with this name already exists", null, new List<string> { "A laptop type with this name already exists" });
                }

                laptopType.Name = updateLaptopTypeDto.Name;
                laptopType.Activite = updateLaptopTypeDto?.Activite;

                _context.LaptopTypes.Update(laptopType);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Laptop type updated successfully", laptopType.Id.ToString());
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error updating laptop type", null, new List<string> { "Error updating laptop type", ex.Message });
            }
        }

        public async Task<ApiResponse<GetAllGetLaptopTypeDto>> GetLaptopTypeByIdAsync(int id)
        {
            try
            {
                var laptopType = await _context.LaptopTypes
                    .Where(t => t.Id == id)
                    .Select(t => new GetAllGetLaptopTypeDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Activite = t.Activite
                    })
                    .FirstOrDefaultAsync();

                if (laptopType == null)
                {
                    return new ApiResponse<GetAllGetLaptopTypeDto>("Laptop type not found", null, new List<string> { "Laptop type not found" });
                }

                return new ApiResponse<GetAllGetLaptopTypeDto>("Laptop type fetched successfully", laptopType);
            }
            catch (DbUpdateException dbEx)
            {
                return new ApiResponse<GetAllGetLaptopTypeDto>("Database error fetching laptop type", null, new List<string> { "Database error fetching laptop type", dbEx.Message });
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetAllGetLaptopTypeDto>("Error fetching laptop type", null, new List<string> { "Error fetching laptop type", ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<GetLaptopTypeDto>>> GetAllLaptopTypesAsync()
        {
            try
            {
                var laptopTypes = await _context.LaptopTypes
                    .Select(t => new GetLaptopTypeDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Activite = t.Activite.HasValue ? t.Activite.Value.ToString() : " "
                    })
                    .ToListAsync();

                if (laptopTypes == null || !laptopTypes.Any())
                {
                    return new ApiResponse<IEnumerable<GetLaptopTypeDto>>("No laptop types found", null, new List<string> { "No laptop types found" });
                }

                return new ApiResponse<IEnumerable<GetLaptopTypeDto>>("Laptop types fetched successfully", laptopTypes);
            }
            catch (DbUpdateException dbEx)
            {
                return new ApiResponse<IEnumerable<GetLaptopTypeDto>>("Database error fetching laptop types", null, new List<string> { "Database error fetching laptop types", dbEx.Message });
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<GetLaptopTypeDto>>("Error fetching laptop types", null, new List<string> { "Error fetching laptop types", ex.Message });
            }
        }


        public async Task<ApiResponse<string>> DeleteLaptopTypeAsync(int id)
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
                var Contract = _context.EmployeeCoos.FirstOrDefault(x => x.IsCancelled == false && x.EndCont >= today && x.Laptop == id);
                if (Contract != null)
                {
                    return new ApiResponse<string>(
                        "Cannot delete this laptop type because it is associated with an active contract.",
                        null,
                        new List<string> { "Laptop type is associated with an active contract." }
                    );
                }

                var laptopType = await _context.LaptopTypes.FindAsync(id);
                if (laptopType == null)
                {
                    return new ApiResponse<string>(
                        "Laptop type not found",
                        null,
                        new List<string> { "Laptop type not found with the given ID." }
                    );
                }

                bool isLaptopTypeInUse = await _context.LaptopRents.AnyAsync(lr => lr.LaptopType == id);
                if (isLaptopTypeInUse)
                {
                    return new ApiResponse<string>(
                        "Cannot delete this laptop type as it is in use in LaptopRent records.",
                        null,
                        new List<string> { "Laptop type is referenced in LaptopRent table." }
                    );
                }

                _context.LaptopTypes.Remove(laptopType);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Laptop type deleted successfully", null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    "Error deleting laptop type",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<string>> UpdateLaptopRentAsync(UpdateLaptopRentDto updateLaptopRentDto)
        {
            try
            {
                var laptopRent = await _context.LaptopRents.FindAsync(updateLaptopRentDto.Id);
                if (laptopRent == null)
                {
                    return new ApiResponse<string>(
                        "LaptopRent not found",
                        null,
                        new List<string> { "LaptopRent record not found with the provided ID." }
                    );
                }

                if (updateLaptopRentDto.Price <= 0)
                {
                    return new ApiResponse<string>(
                        "Invalid price",
                        null,
                        new List<string> { "Price must be greater than zero." }
                    );
                }

                bool laptopTypeExists = await _context.LaptopTypes.AnyAsync(t => t.Id == updateLaptopRentDto.LaptopType);
                if (!laptopTypeExists)
                {
                    return new ApiResponse<string>(
                        "Invalid LaptopType",
                        null,
                        new List<string> { "Laptop type does not exist." }
                    );
                }

                laptopRent.Year = updateLaptopRentDto.Year;
                laptopRent.Month = updateLaptopRentDto.Month;
                laptopRent.Price = updateLaptopRentDto.Price;
                laptopRent.LaptopType = updateLaptopRentDto.LaptopType;

                _context.LaptopRents.Update(laptopRent);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("LaptopRent updated successfully", laptopRent.Id.ToString());
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    "Error updating LaptopRent",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<string>> DeleteLaptopRentAsync(int id)
        {
            try
            {
                var laptopRent = await _context.LaptopRents.FindAsync(id);
                if (laptopRent == null)
                {
                    return new ApiResponse<string>(
                        "LaptopRent not found",
                        null,
                        new List<string> { "LaptopRent record with the given ID does not exist." }
                    );
                }

                _context.LaptopRents.Remove(laptopRent);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("LaptopRent deleted successfully", null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    "Error deleting LaptopRent",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }


    }
}
