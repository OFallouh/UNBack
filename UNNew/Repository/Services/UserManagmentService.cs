using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.DTOS.UserDTO;
using UNNew.Response;
using AutoMapper;
using Konscious.Security.Cryptography;
using UNNew.DTOS.RoleDtos;
using UNNew.DTOS.UserDtos;
using System.Linq.Expressions;
using System.Linq;
using UNNew.Filters;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using UNNew.DTOS.CooDtos;
using UNNew.DTOS.InvoiceDto;

namespace UNNew.Repository.Services
{
    public class UserManagmentService : IUserManagmentService
    {
       
        
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public ApiResponse<string> LoginAccount(LoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == loginDto.UserName);

            if (user != null)
            {
                var isPasswordValid = VerifyArgon2Hash(loginDto.Password, user.PasswordHash);

                if (isPasswordValid)
                {
                    // إذا كانت كلمة المرور صحيحة، بناء التوكن (JWT)
                    var userDto = new LoginUserDto { UserId = user.Id, UserName = user.UserName };
                    var userRole = _context.UserRoles.Include(x => x.Role).FirstOrDefault(x => x.UserId == user.Id);

                    if (userRole != null)
                    {
                        var roleName = userRole.Role.Name;
                        var tokenString = BuildToken(userDto, _configuration["JWT:Key"], roleName);
                        return new ApiResponse<string>("Login successful", tokenString, null);
                    }
                    else
                    {
                        return new ApiResponse<string>("Invalid username or password", null, new List<string> { "User role not found" });
                    }
                }
                else
                {
                    return new ApiResponse<string>("Invalid username or password", null, new List<string> { "Incorrect password" });
                }
            }

            return new ApiResponse<string>("Invalid username or password", null, new List<string> { "User not found" });
        }
        public ApiResponse<string> CreateUser(AddUserDto createUserDto)
        {
            try
            {
               
                if (createUserDto.RoleId == null )
                {
                    return new ApiResponse<string>("Roles are required", null, new List<string> { "Roles are required" });
                }
                var userexists = _context.Users.FirstOrDefault(x => x.UserName.ToLower() == createUserDto.UserName.ToLower());
                if (userexists != null)
                    return new ApiResponse<string>("User already exists", null, new List<string> { "User already exists" });

                // توليد تجزئة كلمة المرور باستخدام Argon2
                var hashedPassword = GenerateArgon2Hash(createUserDto.Password);

                // إنشاء الكائن User
                var user = new User
                {
                    UserName = createUserDto.UserName,
                    PasswordHash = hashedPassword,
                    Email=createUserDto.Email,
                    Permissions=createUserDto.Permissions
                };

                // إضافة الأدوار إلى المستخدم
               
                var role = _context.Roles.FirstOrDefault(r => r.Id == createUserDto.RoleId);
                if (role != null)
                {
                    user.UserRoles.Add(new UserRole
                    {
                        RoleId = role.Id,
                        UserId = user.Id
                    });
                }
                else
                {
                    return new ApiResponse<string>("Role not found", null, new List<string> { "Role not found" });
                }
                

                // إضافة المستخدم إلى قاعدة البيانات
                _context.Users.Add(user);
                _context.SaveChanges();

                // إرجاع استجابة بنجاح العملية مع Id المستخدم
                return new ApiResponse<string>("User created successfully", user.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                // إرجاع رسالة خطأ مع تفاصيل الاستثناء
                return new ApiResponse<string>("Error occurred while creating user", null, new List<string> { ex.Message });
            }
        }
        public ApiResponse<List<UserDto>> GetAllUsers(FilterModel filterRequest)
        {
            try
            {
                // استرجاع جميع المستخدمين مع دورهم
                var query = _context.Users
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .AsQueryable();

                // تطبيق الفلترة العامة (Global Filter)
                if (!string.IsNullOrEmpty(filterRequest.GlobalFilter))
                {
                    var searchTerm = filterRequest.GlobalFilter.ToLower();
                    query = query.Where(user =>
                        user.UserName.ToLower().Contains(searchTerm) ||
                        user.Email.ToLower().Contains(searchTerm) ||
                        (user.UserRoles.Any(ur => ur.Role != null && ur.Role.Name.ToLower().Contains(searchTerm)))
                    );
                }

                // تطبيق الفلاتر المحددة (Column Filters)
                if (filterRequest.Filters != null && filterRequest.Filters.Any())
                {
                    foreach (var filter in filterRequest.Filters)
                    {
                        string field = filter.Key;
                        string filterJsonString = System.Text.Json.JsonSerializer.Serialize(filter.Value);
                        JsonElement filterValue = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(filterJsonString);

                        if (filterValue.TryGetProperty("Value", out JsonElement valueElement))
                        {
                            if (valueElement.ValueKind == JsonValueKind.Null ||
                                (valueElement.ValueKind == JsonValueKind.String && string.IsNullOrEmpty(valueElement.GetString())))
                            {
                                continue;
                            }

                            string value = valueElement.GetString().ToLower();

                            if (filterValue.TryGetProperty("MatchMode", out JsonElement matchModeElement))
                            {
                                string matchMode = matchModeElement.GetString();

                                switch (field.ToLower())
                                {
                                    case "username":
                                        switch (matchMode.ToLower())
                                        {
                                            case "contains":
                                                query = query.Where(u => u.UserName.ToLower().Contains(value));
                                                break;
                                            case "startswith":
                                                query = query.Where(u => u.UserName.ToLower().StartsWith(value));
                                                break;
                                            case "endswith":
                                                query = query.Where(u => u.UserName.ToLower().EndsWith(value));
                                                break;
                                            case "equals":
                                                query = query.Where(u => u.UserName.ToLower() == value);
                                                break;
                                            case "notequals":
                                            case "not_equals":
                                                query = query.Where(u => u.UserName.ToLower() != value);
                                                break;
                                            case "notcontains":
                                                query = query.Where(u => !u.UserName.ToLower().Contains(value));
                                                break;
                                        }
                                        break;
                                    case "email":
                                        switch (matchMode.ToLower())
                                        {
                                            case "contains":
                                                query = query.Where(u => u.Email.ToLower().Contains(value));
                                                break;
                                            case "startswith":
                                                query = query.Where(u => u.Email.ToLower().StartsWith(value));
                                                break;
                                            case "endswith":
                                                query = query.Where(u => u.Email.ToLower().EndsWith(value));
                                                break;
                                            case "equals":
                                                query = query.Where(u => u.Email.ToLower() == value);
                                                break;
                                            case "notequals":
                                            case "not_equals":
                                                query = query.Where(u => u.Email.ToLower() != value);
                                                break;
                                            case "notcontains":
                                                query = query.Where(u => !u.Email.ToLower().Contains(value));
                                                break;
                                        }
                                        break;
                                    case "rolename":
                                        switch (matchMode.ToLower())
                                        {
                                            case "contains":
                                                query = query.Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name.ToLower().Contains(value)));
                                                break;
                                            case "startswith":
                                                query = query.Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name.ToLower().StartsWith(value)));
                                                break;
                                            case "endswith":
                                                query = query.Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name.ToLower().EndsWith(value)));
                                                break;
                                            case "equals":
                                                query = query.Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name.ToLower() == value));
                                                break;
                                            case "notequals":
                                            case "not_equals":
                                                query = query.Where(u => !u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name.ToLower() == value));
                                                break;
                                            case "notcontains":
                                                query = query.Where(u => !u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name.ToLower().Contains(value)));
                                                break;
                                        }
                                        break;
                                        // يمكنك إضافة المزيد من الحالات لأنواع الحقول الأخرى هنا
                                }
                            }
                        }
                    }
                }

                int Count = query.Count();

                // تطبيق الترتيب (Sorting)
                if (filterRequest?.MultiSortMeta?.Any() == true)
                {
                    IOrderedQueryable<User> orderedQuery = null;

                    foreach (var sort in filterRequest.MultiSortMeta)
                    {
                        var field = sort.Field;
                        var sortOrder = sort.Order;

                        switch (field.ToLower())
                        {
                            case "username":
                                orderedQuery = sortOrder == 1
                                    ? (orderedQuery == null ? query.OrderBy(u => u.UserName) : orderedQuery.ThenBy(u => u.UserName))
                                    : (orderedQuery == null ? query.OrderByDescending(u => u.UserName) : orderedQuery.ThenByDescending(u => u.UserName));
                                break;
                            case "email":
                                orderedQuery = sortOrder == 1
                                    ? (orderedQuery == null ? query.OrderBy(u => u.Email) : orderedQuery.ThenBy(u => u.Email))
                                    : (orderedQuery == null ? query.OrderByDescending(u => u.Email) : orderedQuery.ThenByDescending(u => u.Email));
                                break;
                            case "rolename":
                                orderedQuery = sortOrder == 1
                                    ? (orderedQuery == null ? query.OrderBy(u => u.UserRoles.FirstOrDefault().Role.Name) : orderedQuery.ThenBy(u => u.UserRoles.FirstOrDefault().Role.Name))
                                    : (orderedQuery == null ? query.OrderByDescending(u => u.UserRoles.FirstOrDefault().Role.Name) : orderedQuery.ThenByDescending(u => u.UserRoles.FirstOrDefault().Role.Name));
                                break;
                                // يمكنك إضافة المزيد من الحالات لأنواع الحقول الأخرى للترتيب
                        }
                    }
                    if (orderedQuery != null)
                    {
                        query = orderedQuery;
                    }
                }
                   
                // تطبيق الترقيم (Pagination)
                int defaultPageSize = 5; // عدد الصفحات الافتراضية
                int pageSize = filterRequest.Rows > 0 ? filterRequest.Rows : defaultPageSize;
                int skip = filterRequest.First > 0 ? filterRequest.First : 0;

                query = query.Skip(skip).Take(pageSize);

                // تحويل النتيجة إلى UserDto
                var users = query.Select(user => new UserDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Permissions=user.Permissions,
                    RoleId = user.UserRoles.FirstOrDefault(x => x.UserId == user.Id) != null ? user.UserRoles.FirstOrDefault(x => x.UserId == user.Id).RoleId : null,
                    RoleName = user.UserRoles.FirstOrDefault(x => x.UserId == user.Id) != null && user.UserRoles.FirstOrDefault(x => x.UserId == user.Id).Role != null ? user.UserRoles.FirstOrDefault(x => x.UserId == user.Id).Role.Name : null
                }).ToList();

                return new ApiResponse<List<UserDto>>("Users and roles retrieved successfully", users, null, Count);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<UserDto>>("Error occurred while retrieving users and roles", null, new List<string> { ex.Message });
            }
        }

        // لا تحتاج إلى استدعاء ApplyStringFilter هنا بشكل مباشر بعد الآن.
        // احتفظ بتعريف ApplyStringFilter والدوال المساعدة الأخرى إذا كنت تستخدمها في أماكن أخرى.
        //public ApiResponse<List<UserDto>> GetAllUsers(FilterModel filterRequest)
        //{
        //    try
        //    {
        //        // استرجاع جميع المستخدمين مع دورهم الوحيد
        //        var query = _context.Users
        //            .Include(x => x.UserRoles)
        //            .ThenInclude(x => x.Role)
        //            .AsQueryable(); // استخدم AsQueryable لتطبيق الفلاتر لاحقاً
        //        int Count = query.ToList().Count;
        //        // تطبيق الفلاتر
        //        if (filterRequest.Filters != null)
        //        {
        //            foreach (var filter in filterRequest.Filters)
        //            {
        //                var field = filter.Key;
        //                var criteria = filter.Value;

        //                if (criteria != null && !string.IsNullOrEmpty(criteria.Value))
        //                {
        //                    switch (field.ToLower())
        //                    {
        //                        case "username":
        //                            query = ApplyStringFilter(query, x => x.UserName, criteria);
        //                            break;
        //                        case "rolename":
        //                            query = ApplyStringFilter(query, x => x.UserRoles.FirstOrDefault(ur => ur.UserId == x.Id).Role.Name, criteria);
        //                            break;
        //                        case "email":
        //                            query = ApplyStringFilter(query, x => x.Email, criteria);
        //                            break;
        //                            // يمكن إضافة المزيد من الفلاتر هنا
        //                    }
        //                }
        //            }
        //        }

        //        // تطبيق الفلتر العام
        //        if (!string.IsNullOrEmpty(filterRequest.GlobalFilter))
        //        {
        //            query = query.Where(x =>
        //                x.UserName.Contains(filterRequest.GlobalFilter) ||
        //                x.UserRoles.Any(ur => ur.Role.Name.Contains(filterRequest.GlobalFilter))
        //            );
        //        }

        //        // تطبيق الفرز
        //        if (filterRequest.MultiSortMeta != null && filterRequest.MultiSortMeta.Any())
        //        {
        //            foreach (var sortMeta in filterRequest.MultiSortMeta)
        //            {
        //                switch (sortMeta.Field.ToLower())
        //                {
        //                    case "username":
        //                        query = sortMeta.Order == 1 ? query.OrderBy(x => x.UserName) : query.OrderByDescending(x => x.UserName);
        //                        break;
        //                    case "rolename":
        //                        query = sortMeta.Order == 1 ? query.OrderBy(x => x.UserRoles.FirstOrDefault(ur => ur.UserId == x.Id).Role.Name) : query.OrderByDescending(x => x.UserRoles.FirstOrDefault(ur => ur.UserId == x.Id).Role.Name);
        //                        break;
        //                    case "email":
        //                        query = sortMeta.Order == 1 ? query.OrderBy(x => x.Email) : query.OrderByDescending(x => x.Email);
        //                        break;
        //                        // يمكن إضافة المزيد من عمليات الفرز هنا
        //                }
        //            }
        //        }

        //        // تطبيق الترقيم (Pagination)
        //        int defaultPageSize = 5; // عدد الصفحات الافتراضية
        //        int pageSize = filterRequest.Rows > 0 ? filterRequest.Rows : defaultPageSize;
        //        int skip = filterRequest.First > 0 ? filterRequest.First : 0;

        //        query = query.Skip(skip).Take(pageSize);

        //        // تحويل النتيجة إلى UserDto
        //        var users = query.Select(user => new UserDto
        //        {
        //            UserId = user.Id,
        //            UserName = user.UserName,
        //            Email=user.Email,
        //            RoleId = user.UserRoles.FirstOrDefault(x => x.UserId == user.Id) != null ? user.UserRoles.FirstOrDefault(x => x.UserId == user.Id).RoleId : null,
        //            RoleName = user.UserRoles.FirstOrDefault(x => x.UserId == user.Id) != null && user.UserRoles.FirstOrDefault(x => x.UserId == user.Id).Role != null ? user.UserRoles.FirstOrDefault(x => x.UserId == user.Id).Role.Name : null
        //        }).ToList();

        //        return new ApiResponse<List<UserDto>>("Users and roles retrieved successfully", users, null, Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse<List<UserDto>>("Error occurred while retrieving users and roles", null, new List<string> { ex.Message });
        //    }
        //}


        //// دالة مساعدة لتطبيق فلاتر السلاسل النصية
        //private IQueryable<User> ApplyStringFilter(IQueryable<User> query, Expression<Func<User, string>> property, FilterCriteria criteria)
        //{
        //    switch (criteria.MatchMode.ToLower())
        //    {
        //        case "contains":
        //            return query.Where(Expression.Lambda<Func<User, bool>>(
        //                Expression.Call(property.Body, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(criteria.Value)),
        //                property.Parameters));
        //        case "startswith":
        //            return query.Where(Expression.Lambda<Func<User, bool>>(
        //                Expression.Call(property.Body, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(criteria.Value)),
        //                property.Parameters));
        //        case "endswith":
        //            return query.Where(Expression.Lambda<Func<User, bool>>(
        //                Expression.Call(property.Body, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(criteria.Value)),
        //                property.Parameters));
        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<User, bool>>(
        //                Expression.Equal(property.Body, Expression.Constant(criteria.Value)),
        //                property.Parameters));
        //        case "notequals":
        //            return query.Where(Expression.Lambda<Func<User, bool>>(
        //                Expression.NotEqual(property.Body, Expression.Constant(criteria.Value)),
        //                property.Parameters));
        //        case "notcontains":
        //            return query.Where(Expression.Lambda<Func<User, bool>>(
        //                Expression.Not(Expression.Call(property.Body, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(criteria.Value))),
        //                property.Parameters));
        //        default:
        //            return query;
        //    }
        //}
        public ApiResponse<UserDto> GetUserById(int userId)
        {
            try
            {
                var user = _context.Users
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .Where(u => u.Id == userId)
                    .Select(user => new UserDto
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Permissions=user.Permissions,
                        RoleId = user.UserRoles.FirstOrDefault(x => x.UserId == user.Id) != null ? user.UserRoles.FirstOrDefault(x => x.UserId == user.Id).RoleId : null,
                        RoleName = user.UserRoles.FirstOrDefault(x => x.UserId == user.Id) != null && user.UserRoles.FirstOrDefault(x => x.UserId == user.Id).Role != null ? user.UserRoles.FirstOrDefault(x => x.UserId == user.Id).Role.Name : null
                    })
                    .FirstOrDefault();

                if (user == null)
                {
                    return new ApiResponse<UserDto>($"User with ID {userId} not found", null, new List<string> { "User not found" });
                }

                return new ApiResponse<UserDto>("User retrieved successfully", user, null);
            }
            catch (DbUpdateException ex)
            {
                // معالجة أخطاء قاعدة البيانات
                // _logger.LogError(ex, "Database error occurred while retrieving user with ID {UserId}", userId);
                return new ApiResponse<UserDto>("Database error occurred while retrieving user", null, new List<string> { ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                // معالجة الأخطاء العامة
                // _logger.LogError(ex, "Error occurred while retrieving user with ID {UserId}", userId);
                return new ApiResponse<UserDto>("Error occurred while retrieving user", null, new List<string> { ex.Message });
            }
        }
        public ApiResponse<string> DeleteUser(int userId)
        {
            try
            {
                // البحث عن المستخدم باستخدام الـ userId
                var user = _context.Users.FirstOrDefault(x => x.Id == userId);

                if (user == null)
                {
                    return new ApiResponse<string>("User not found", null, new List<string> { "The user does not exist." });
                }

                // حذف المستخدم
                _context.Users.Remove(user);
                _context.SaveChanges();

                // إرجاع استجابة بنجاح الحذف
                return new ApiResponse<string>("User deleted successfully", null, null);
            }
            catch (Exception ex)
            {
                // إرجاع رسالة خطأ إذا حدث استثناء
                return new ApiResponse<string>("Error occurred while deleting user", null, new List<string> { ex.Message });
            }
        }
        public ApiResponse<string> UpdateUser(UpdateUserDto updateUser)
        {
            try
            {
                // البحث عن المستخدم باستخدام الـ UserId
                var user = _context.Users.FirstOrDefault(x => x.Id == updateUser.UserId);

                if (user == null)
                {
                    return new ApiResponse<string>("User not found", null, new List<string> { "The user does not exist." });
                }

                // التحقق من وجود اسم المستخدم
                bool userNameExists = _context.Users.Any(x => x.UserName == updateUser.UserName && x.Id != updateUser.UserId);
                if (userNameExists)
                {
                    return new ApiResponse<string>("Username already exists", null, new List<string> { "A user with the same username already exists." });
                }

                user.UserName = updateUser.UserName;
                user.Email = updateUser.Email;
                user.Permissions = updateUser.Permissions;

                   
                // البحث عن دور المستخدم
                var role = _context.UserRoles.FirstOrDefault(x => x.UserId == updateUser.UserId);

                if (role != null)
                {
                    role.RoleId = updateUser.RoleId;
                    _context.UserRoles.Update(role);
                }

                // حفظ جميع التغييرات مرة واحدة
                _context.SaveChanges();

                return new ApiResponse<string>("User updated successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while updating user", null, new List<string> { ex.Message });
            }
        }
        public bool VerifyArgon2Hash(string password, string storedHash)
        {
            // تحويل التجزئة المخزنة من Base64 إلى بايتات
            var storedHashBytes = Convert.FromBase64String(storedHash);

            // استخدام نفس معايير Argon2 التي استخدمتها عند إنشاء التجزئة
            using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                hasher.DegreeOfParallelism = 8;  // عدد الخيوط (Threads)
                hasher.MemorySize = 65536;  // حجم الذاكرة (بالكيلوبايت)
                hasher.Iterations = 4;  // عدد التكرارات (Iterations)

                // توليد التجزئة الجديدة بناءً على كلمة المرور المدخلة
                var hashBytes = hasher.GetBytes(32);  // طول التجزئة المطلوبة (بالبايت)

                // مقارنة التجزئة المخزنة بالتجزئة الناتجة عن كلمة المرور المدخلة
                return storedHashBytes.SequenceEqual(hashBytes);
            }
        }
        private string BuildToken(LoginUserDto user, string secretKey, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // ✅ UserId
                new Claim(JwtRegisteredClaimNames.FamilyName, user.UserName),   // ✅ UserName
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ✅ Unique Token ID
                new Claim("role", role), // ✅ جعل الدور مثل sub وليس داخل ClaimTypes.Role
                new Claim("expDate", DateTime.UtcNow.AddDays(7).ToString("dd/MM/yyyy HH:mm")) // ✅ تاريخ انتهاء التوكن مع الساعة والدقيقة
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:44387/",
                audience: "https://localhost:44387/",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GenerateArgon2Hash(string password)
        {
            // ضبط معايير Argon2: الوقت (iterations)، الذاكرة، وعدد الخيوط
            using (var hasher = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                hasher.DegreeOfParallelism = 8; // عدد الخيوط
                hasher.MemorySize = 65536; // حجم الذاكرة (بالكيلوبايت)
                hasher.Iterations = 4; // عدد التكرار

                var hashBytes = hasher.GetBytes(32); // طول التجزئة (بالبايت)
                return Convert.ToBase64String(hashBytes); // إرجاع التجزئة في صيغة Base64
            }
        }
        public ApiResponse<string> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == changePasswordDto.UserId);

                if (user == null)
                {
                    return new ApiResponse<string>("User not found", null, new List<string> { "User does not exist." });
                }

                // التحقق من كلمة المرور القديمة
                var isPasswordValid = VerifyArgon2Hash(changePasswordDto.OldPassword, user.PasswordHash);

                if (!isPasswordValid)
                {
                    return new ApiResponse<string>("Old password is incorrect", null, new List<string> { "The old password is incorrect" });
                }

                // توليد التجزئة لكلمة المرور الجديدة
                var hashedNewPassword = GenerateArgon2Hash(changePasswordDto.NewPassword);
                user.PasswordHash = hashedNewPassword;

                // حفظ التغييرات في قاعدة البيانات
                _context.Users.Update(user);
                _context.SaveChanges();

                return new ApiResponse<string>("Password changed successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while changing password", null, new List<string> { ex.Message });
            }
        }
        public ApiResponse<string> ResetPasswordForAdmin(ResetPasswordDto resetPasswordDto )
        {
            try
            {
                // البحث عن المستخدم
                var user = _context.Users.FirstOrDefault(x => x.Id == resetPasswordDto.userId);
                if (user == null)
                {
                    return new ApiResponse<string>("User not found", null, new List<string> { "The user does not exist." });
                }

                // تشفير كلمة المرور الجديدة
                user.PasswordHash = GenerateArgon2Hash(resetPasswordDto.newPassword);

                // حفظ التغييرات في قاعدة البيانات
                _context.Users.Update(user);
                _context.SaveChanges();

                return new ApiResponse<string>("Password reset successfully", "Password updated for UserId: " + resetPasswordDto.userId, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while resetting password", null, new List<string> { ex.Message });
            }
        }
        public ApiResponse<string> Logout()
        {
            try
            {
                // يمكنك إضافة منطق لتسجيل الخروج هنا إذا كان لديك
                return new ApiResponse<string>("Success", "User logged out successfully", null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while logging out", null, new List<string> { ex.Message });
            }
        }



    }
}
