using AutoMapper;
using UNNew.DTOS.TypeOfContractDtos;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;
using Microsoft.EntityFrameworkCore;

namespace UNNew.Repository.Services
{
    public class TypeOfContractManagmentService : ITypeOfContractManagmentService
    {

        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TypeOfContractManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        // Create a new TypeOfContract
        public async Task<ApiResponse<string>> CreateTypeOfContractAsync(CreateTypeOfContractDto createTypeOfContractDto)
        {
            try
            {
                if (string.IsNullOrEmpty(createTypeOfContractDto.NmaeEn))
                {
                    return new ApiResponse<string>(null, null, new List<string> { "Name in English is required" });
                }

                // Check if a type of contract with the same name already exists
                bool typeExists = await _context.TypeOfContracts.AnyAsync(t => t.NmaeEn == createTypeOfContractDto.NmaeEn);
                if (typeExists)
                {
                    return new ApiResponse<string>(null, null, new List<string> { "A type of contract with this name already exists" });
                }

                // Get the last TypeOfContractId and increment it
                int lastTypeId = await _context.TypeOfContracts.MaxAsync(t => (int?)t.TypeOfContractId) ?? 0;
                int newTypeId = lastTypeId + 1;

                var typeOfContract = new TypeOfContract
                {
                    NmaeEn = createTypeOfContractDto.NmaeEn
                };

                _context.TypeOfContracts.Add(typeOfContract);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Type of contract created successfully", typeOfContract.TypeOfContractId.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(null, null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> UpdateTypeOfContractAsync(UpdateTypeOfContractDto updateTypeOfContractDto)
        {
            try
            {
                var typeOfContract = await _context.TypeOfContracts.FindAsync(updateTypeOfContractDto.TypeOfContractId);
                if (typeOfContract == null)
                {
                    return new ApiResponse<string>(null, null, new List<string> { "Type of contract not found" });
                }

                if (string.IsNullOrEmpty(updateTypeOfContractDto.NmaeEn))
                {
                    return new ApiResponse<string>(null, null, new List<string> { "Name in English is required" });
                }

                // Check if a type of contract with the same name already exists (excluding the current type)
                bool typeExists = await _context.TypeOfContracts.AnyAsync(t => t.NmaeEn == updateTypeOfContractDto.NmaeEn && t.TypeOfContractId != updateTypeOfContractDto.TypeOfContractId);
                if (typeExists)
                {
                    return new ApiResponse<string>(null, null, new List<string> { "A type of contract with this name already exists" });
                }

                typeOfContract.NmaeEn = updateTypeOfContractDto.NmaeEn;

                _context.TypeOfContracts.Update(typeOfContract);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Type of contract updated successfully", typeOfContract.TypeOfContractId.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(null, null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<TypeOfContractDto>> GetTypeOfContractByIdAsync(int typeOfContractId)
        {
            try
            {
                var typeOfContract = await _context.TypeOfContracts
                    .Where(t => t.TypeOfContractId == typeOfContractId)
                    .Select(t => new TypeOfContractDto
                    {
                        TypeOfContractId = t.TypeOfContractId,
                        NmaeEn = t.NmaeEn
                    })
                    .FirstOrDefaultAsync();

                if (typeOfContract == null)
                {
                    return new ApiResponse<TypeOfContractDto>(null, null, new List<string> { "Type of contract not found" });
                }

                return new ApiResponse<TypeOfContractDto>("Type of contract fetched successfully", typeOfContract, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<TypeOfContractDto>(null, null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<TypeOfContractDto>>> GetAllTypeOfContractsAsync()
        {
            try
            {
                var typeOfContracts = await _context.TypeOfContracts
                    .Select(t => new TypeOfContractDto
                    {
                        TypeOfContractId = t.TypeOfContractId,
                        NmaeEn = t.NmaeEn
                    })
                    .ToListAsync();

                if (typeOfContracts == null || !typeOfContracts.Any())
                {
                    return new ApiResponse<IEnumerable<TypeOfContractDto>>(null, null, new List<string> { "No type of contracts found" });
                }

                return new ApiResponse<IEnumerable<TypeOfContractDto>>("Type of contracts fetched successfully", typeOfContracts, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<TypeOfContractDto>>(null, null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> DeleteTypeOfContractAsync(int typeOfContractId)
        {
            try
            {
                var Contract = _context.EmployeeCoos.FirstOrDefault(x => x.TypeOfContractId == typeOfContractId);
                if (Contract != null)
                    return new ApiResponse<string>(null, null, new List<string> { "Type Of Contract is associated with an active contract." });

                var typeOfContract = await _context.TypeOfContracts.FindAsync(typeOfContractId);
                if (typeOfContract == null)
                {
                    return new ApiResponse<string>(null, null, new List<string> { "Type of contract not found" });
                }

                _context.TypeOfContracts.Remove(typeOfContract);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Type of contract deleted successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(null, null, new List<string> { ex.Message });
            }
        }

    }
}



