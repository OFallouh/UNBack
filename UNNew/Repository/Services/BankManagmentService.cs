using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UNNew.DTOS.BankDtos;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class BankManagmentService : IBankManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public BankManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<IEnumerable<UnEmp>> GetAllAsync()
        {
            return await _context.UnEmps.ToListAsync();
        }
        public async Task<ApiResponse<List<BankDto>>> GetAllBanksAsync()
        {
            try
            {
                var banks = await _context.Banks.ToListAsync(); // جلب جميع البنوك
                var bankDtos = banks.Select(bank => new BankDto
                {
                    BanksId = bank.BanksId,
                    BanksName = bank.BanksName,
                    BankLogoUrl= bank.BankLogoUrl,
                }).ToList();

                return new ApiResponse<List<BankDto>>("Banks retrieved successfully", bankDtos, null); // إرجاع استجابة بنجاح
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<BankDto>>("Error occurred while retrieving banks", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<BankDto>> GetBankByIdAsync(int id)
        {
            try
            {
                var bank = await _context.Banks
                    .FirstOrDefaultAsync(b => b.BanksId == id); // جلب البنك باستخدام الـ BanksId

                if (bank == null)
                {
                    return new ApiResponse<BankDto>("Bank not found", null, null); // إرجاع رسالة في حالة عدم العثور على البنك
                }

                var bankDto = new BankDto
                {
                    BanksId = bank.BanksId,
                    BanksName = bank.BanksName,
                    BankLogoUrl = bank.BankLogoUrl,
                };

                return new ApiResponse<BankDto>("Bank retrieved successfully", bankDto, null); // إرجاع استجابة بنجاح
            }
            catch (Exception ex)
            {
                return new ApiResponse<BankDto>("Error occurred while retrieving the bank", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> CreateBankAsync(string BanksName, IFormFile? files)
        {
            try
            {
                // تحقق من وجود بنك بنفس الاسم
                bool bankExists = await _context.Banks.AnyAsync(b => b.BanksName == BanksName);
                if (bankExists)
                {
                    return new ApiResponse<string>("Bank name already exists", null, new List<string> { "A bank with the same name already exists." });
                }

                // الحصول على ID جديد
                int lastBanksId = await _context.Banks.MaxAsync(b => (int?)b.BanksId) ?? 0;
                int newBanksId = lastBanksId + 1;

                // مسار تخزين الشعار
                string logoFolderPath = _configuration["FileStorage:LogoBankUrlPath"];

                // التأكد من وجود المجلد وإنشاؤه إذا غير موجود
                if (!Directory.Exists(logoFolderPath))
                {
                    Directory.CreateDirectory(logoFolderPath);
                }

                string? logoFileName = null;
                string? fullLogoPath = null;

                // تخزين الشعار الجديد إذا الملف موجود
                if (files != null)
                {
                    logoFileName = $"{BanksName}_logo{Path.GetExtension(files.FileName)}";
                    fullLogoPath = Path.Combine(logoFolderPath, logoFileName);

                    // كتابة الملف
                    using (var stream = new FileStream(fullLogoPath, FileMode.Create))
                    {
                        await files.CopyToAsync(stream);
                    }
                }

                // إنشاء الكيان
                var bank = new Bank
                {
                    BanksId = newBanksId,
                    BanksName = BanksName,
                    BankLogoUrl = fullLogoPath ?? string.Empty // تخزين المسار الكامل
                };

                _context.Banks.Add(bank);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Bank created successfully", BanksName, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while creating the bank", null, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<string>> UpdateBankAsync(int id, string BanksName, IFormFile? files)
        {
            try
            {
                
                var bank = await _context.Banks.FirstOrDefaultAsync(b => b.BanksId == id);
                if (bank == null)
                {
                    return new ApiResponse<string>("Bank not found", null, new List<string> { "Bank not found." });
                }

                // تحديث الاسم
                bank.BanksName = BanksName;

                // تحديث اللوغو إن تم إرساله
                if (files != null && files.Length > 0)
                {
                    // مسار حفظ الشعار
                    string logoFolderPath = _configuration["FileStorage:LogoBankUrlPath"];
                    if (!Directory.Exists(logoFolderPath))
                    {
                        Directory.CreateDirectory(logoFolderPath);
                    }

                    // حذف الشعار القديم إذا وُجد
                    if (!string.IsNullOrEmpty(bank.BankLogoUrl) && File.Exists(bank.BankLogoUrl))
                    {
                        File.Delete(bank.BankLogoUrl);
                    }

                    // اسم جديد للملف: اسم البنك + "logo" + الامتداد
                    string newFileName = $"{BanksName}_logo{Path.GetExtension(files.FileName)}";
                    string fullPath = Path.Combine(logoFolderPath, newFileName);

                    // حفظ الملف
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await files.CopyToAsync(stream);
                    }

                    // تحديث المسار في قاعدة البيانات
                    bank.BankLogoUrl = fullPath;
                }

                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Bank updated successfully", BanksName, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while updating the bank", null, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<string>> DeleteBankAsync(int id)
        {
            try
            {
                var bank = await _context.Banks.FindAsync(id); // استخدام FindAsync لزيادة الكفاءة

                if (bank == null)
                {
                    return new ApiResponse<string>(
                        "Bank not found",
                        null,
                        new List<string> { "Bank with the provided ID does not exist." }
                    );
                }

                // التحقق من وجود موظفين مرتبطين
                bool hasRelatedEmployees = await _context.UnEmps.AnyAsync(emp => emp.BankId == id);
                if (hasRelatedEmployees)
                {
                    return new ApiResponse<string>(
                        "Cannot delete bank because it is associated with employees",
                        null,
                        new List<string> { "Employees are linked to this bank." }
                    );
                }

                _context.Banks.Remove(bank);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Bank deleted successfully", null, null);
            }
            catch (Exception ex)
            {
                // Logging can be added here if needed
                return new ApiResponse<string>(
                    "Error occurred while deleting the bank",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }


    }
}
