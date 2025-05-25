using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using UNNew.DTOS.UNEmployeeDtos;
using UNNew.Filters;
using UNNew.Helpers;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;
using Microsoft.AspNetCore.Hosting;
using UNNew.DTOS.FileDto;
using System.Diagnostics.Contracts;
using UNNew.DTOS.CooDtos;
using UNNew.DTOS.InvoiceDto;


namespace UNNew.Repository.Services
{
    public class UNEmployeeManagmentService : IUNEmployeeManagmentService
    {

        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UNEmployeeManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        // Create
        public async Task<ApiResponse<string>> CreateUNEmployeeAsync(CreateUNEmployeeDto createUNEmployeeDto)
        {
            try
            {
                // التحقق من تكرار الاسم الثلاثي واسم الأم
                bool isNameExists = await _context.UnEmps.AnyAsync(e =>
                    e.EmpName == createUNEmployeeDto.personal.EmpName &&
                    e.FatherNameArabic == createUNEmployeeDto.personal.FatherNameArabic &&
                    e.MotherNameArabic == createUNEmployeeDto.personal.MotherNameArabic
                );

                if (isNameExists)
                {
                    return new ApiResponse<string>("Employee with the same full name and mother's name already exists", null, new List<string> { "Employee with the same full name and mother's name already exists." });
                }

                // التحقق من تكرار الرقم الوطني
                bool isIdExists = await _context.UnEmps.AnyAsync(e =>
                    e.IdNo == createUNEmployeeDto.personal.IdNo
                );

                if (isIdExists)
                {
                    return new ApiResponse<string>("Employee with the same national ID already exists", null, new List<string> { "Employee with the same national ID already exists." });
                }

                // تحويل DTO إلى Model
                var UNEmployee = _mapper.Map<UnEmp>(createUNEmployeeDto);

                // إضافة الموظف إلى قاعدة البيانات
                _context.UnEmps.Add(UNEmployee);
                await _context.SaveChangesAsync();

                // إرجاع استجابة ناجحة
                return new ApiResponse<string>("Employee created successfully", UNEmployee.RefNo.ToString(), null);
            }
            catch (Exception ex)
            {
                // إرجاع استجابة خطأ مع رسالة في الخاصية error
                return new ApiResponse<string>(null, null, new List<string> { ex.Message });
            }
        }


        // Update
        public async Task<ApiResponse<string>> UpdateUNEmployeeAsync(UpdateUNEmployeeDto updatedEmployee, int Id)
        {
            try
            {
                // تحقق مما إذا كان الموظف موجودًا
                var existingEmployee = await _context.UnEmps.FirstOrDefaultAsync(e => e.RefNo == Id);

                if (existingEmployee == null)
                {
                    return new ApiResponse<string>("The employee with the specified RefNo does not exist", null, new List<string> { "The employee with the specified RefNo does not exist." });
                }

                // التحقق من الاسم الثلاثي مع اسم الأم (باستثناء نفس الموظف)
                bool isNameExists = await _context.UnEmps.AnyAsync(e =>
                    e.RefNo != Id && // استثناء نفس الموظف
                    e.EmpName == updatedEmployee.personal.EmpName &&
                    e.FatherNameArabic == updatedEmployee.personal.FatherNameArabic &&
                    e.MotherNameArabic == updatedEmployee.personal.MotherNameArabic
                );

                if (isNameExists)
                {
                    return new ApiResponse<string>("Another employee with the same full name and mother's name already exists", null, new List<string> { "Another employee with the same full name and mother's name already exists." });
                }

                // التحقق من الرقم الوطني (باستثناء نفس الموظف)
                bool isIdExists = await _context.UnEmps.AnyAsync(e =>
                    e.RefNo != Id && // استثناء نفس الموظف
                    e.IdNo == updatedEmployee.personal.IdNo
                );

                if (isIdExists)
                {
                    return new ApiResponse<string>("Another employee with the same national ID already exists", null, new List<string> { "Another employee with the same national ID already exists." });
                }

                // التحديث باستخدام الـ Mapper
                _mapper.Map(updatedEmployee, existingEmployee);
                existingEmployee.RefNo = Id;

                _context.Entry(existingEmployee).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Employee updated successfully", existingEmployee.RefNo.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(null, null, new List<string> { ex.Message });
            }
        }


        // Delete
        public async Task<ApiResponse<bool>> DeleteUNEmployeeAsync(int UNEmployeeId)
        {
            try
            {
                var employee = await _context.UnEmps.FindAsync(UNEmployeeId);
                if (employee == null)
                {
                    // إرجاع رسالة الخطأ في خاصية error بدلاً من message
                    return new ApiResponse<bool>("The employee with the specified ID does not exist", false, new List<string> { "The employee with the specified ID does not exist." });
                }

                employee.IsDeleted = true;
                _context.UnEmps.Update(employee);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>("Employee deleted successfully", true, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(null, false, new List<string> { ex.Message });
            }
        }


        // Disable (Soft delete)
        public async Task<ApiResponse<bool>> DisableUNEmployeeAsync(int id)
        {
            try
            {
                var employee = await _context.UnEmps.FindAsync(id);
                if (employee == null)
                {
                    return new ApiResponse<bool>("The employee with the specified ID does not exist", false, new List<string> { "The employee with the specified ID does not exist." });
                }

                employee.Active = !employee.Active;
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>("Employee disabled successfully", true, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(null, false, new List<string> { ex.Message });
            }
        }

        // GetAll
        //public async Task<ApiResponse<List<UNEmployeeDto>>> GetAllEmployeesAsync(FilterModel filterModel)
        //{
        //    try
        //    {
        //        var query = _context.UnEmps.AsQueryable();
        //        var Count = query.ToList().Count();

        //        // Apply global filter
        //        if (!string.IsNullOrEmpty(filterModel.GlobalFilter))
        //        {
        //            query = query.Where(emp =>
        //                emp.EmpName.Contains(filterModel.GlobalFilter) ||
        //                emp.ArabicName.Contains(filterModel.GlobalFilter) ||
        //                emp.EmailAddress.Contains(filterModel.GlobalFilter) ||
        //                emp.MobileNo.ToString().Contains(filterModel.GlobalFilter) ||
        //                emp.Gender.Contains(filterModel.GlobalFilter));
        //        }

        //        // Apply individual filters
        //        if (filterModel.Filters != null)
        //        {
        //            foreach (var filter in filterModel.Filters)
        //            {
        //                var propertyName = filter.Key;
        //                var filterCriteria = filter.Value;

        //                if (filterCriteria.Value != null)
        //                {
        //                    // جعل matchMode غير حساس لحالة الأحرف
        //                    filterCriteria.MatchMode = filterCriteria.MatchMode?.ToLower();

        //                    // جعل أسماء الخصائص غير حساسة لحالة الأحرف
        //                    switch (propertyName.ToLower())
        //                    {
        //                        case "refno":
        //                            query = ApplyNumericFilter(query, emp => emp.RefNo, filterCriteria);
        //                            break;
        //                        case "empname":
        //                            query = ApplyStringFilter(query, emp => emp.EmpName, filterCriteria);
        //                            break;
        //                        case "arabicname":
        //                            query = ApplyStringFilter(query, emp => emp.ArabicName, filterCriteria);
        //                            break;
        //                        case "mothernamearabic":
        //                            query = ApplyStringFilter(query, emp => emp.MotherNameArabic, filterCriteria);
        //                            break;
        //                        case "fathernamearabic":
        //                            query = ApplyStringFilter(query, emp => emp.FatherNameArabic, filterCriteria);
        //                            break;
        //                        case "idno":
        //                            query = ApplyStringFilter(query, emp => emp.IdNo, filterCriteria);
        //                            break;
        //                        case "emailaddress":
        //                            query = ApplyStringFilter(query, emp => emp.EmailAddress, filterCriteria);
        //                            break;
        //                        case "mobileno":
        //                            query = ApplyNumericFilter(query, emp => emp.MobileNo, filterCriteria);
        //                            break;
        //                        case "gender":
        //                            query = ApplyStringFilter(query, emp => emp.Gender, filterCriteria);
        //                            break;
        //                        case "teamname":
        //                            query = ApplyStringFilter(query, emp => emp.TeamNavigation.TeamName ?? "N/A", filterCriteria);
        //                            break;
        //                        case "cityname":
        //                            query = ApplyStringFilter(query, emp => emp.CityNavigation.NameEn ?? "N/A", filterCriteria);
        //                            break;
        //                        case "coonumber":
        //                            query = ApplyStringFilter(query, emp => emp.Coo.CooNumber ?? "N/A", filterCriteria);
        //                            break;
        //                        case "typeofcontractname":
        //                            query = ApplyStringFilter(query, emp => emp.TypeOfContractNavigation.NmaeEn ?? "N/A", filterCriteria);
        //                            break;
        //                        case "contractsigned":
        //                            query = ApplyBooleanFilter(query, emp => emp.ContractSigned, filterCriteria);
        //                            break;
        //                        case "contractstartdate":
        //                            query = ApplyDateFilter(query, emp => emp.ContractStartDate, filterCriteria);
        //                            break;
        //                        case "contractenddate":
        //                            query = ApplyDateFilter(query, emp => emp.ContractEndDate, filterCriteria);
        //                            break;
        //                        case "salary":
        //                            query = ApplyNumericFilter(query, emp => emp.Salary, filterCriteria);
        //                            break;
        //                        case "transportation":
        //                            query = ApplyBooleanFilter(query, emp => emp.Transportation, filterCriteria);
        //                            break;
        //                        case "ismobile":
        //                            query = ApplyBooleanFilter(query, emp => emp.IsMobile, filterCriteria);
        //                            break;
        //                        case "insurancelife":
        //                            query = ApplyBooleanFilter(query, emp => emp.InsuranceLife, filterCriteria);
        //                            break;
        //                        case "startlifedate":
        //                            query = ApplyStartLifeDateFilter(query, filterCriteria);
        //                            break;

        //                        case "endlifedate":
        //                            query = ApplyEndLifeDateFilter(query, filterCriteria);
        //                            break;
        //                        case "insurancemedical":
        //                            query = ApplyBooleanFilter(query, emp => emp.InsuranceMedical, filterCriteria);
        //                            break;
        //                        case "supervisor":
        //                            query = ApplyStringFilter(query, emp => emp.SuperVisor, filterCriteria);
        //                            break;
        //                        case "areamanager":
        //                            query = ApplyStringFilter(query, emp => emp.AreaManager, filterCriteria);
        //                            break;
        //                        case "projectname":
        //                            query = ApplyStringFilter(query, emp => emp.ProjectName, filterCriteria);
        //                            break;
        //                        case "laptoptype":
        //                            query = ApplyStringFilter(query, emp => emp.LaptopNavigation.Name ?? "N/A", filterCriteria);
        //                            break;
        //                        case "clientname":
        //                            query = ApplyStringFilter(query, emp => emp.Client.ClientName ?? "N/A", filterCriteria);
        //                            break;
        //                        case "medicalcheck":
        //                            query = ApplyBooleanFilter(query, emp => emp.MedicalCheck ?? false, filterCriteria);
        //                            break;
        //                        case "oldemployment":
        //                            query = ApplyBooleanFilter(query, emp => emp.OldEmployment ?? false, filterCriteria);
        //                            break;
        //                        case "securitycheck":
        //                            query = ApplyBooleanFilter(query, emp => emp.SecurityCheck ?? false, filterCriteria);
        //                            break;
        //                        case "bankname":
        //                            query = ApplyStringFilter(query, emp => emp.Bank.BanksName ?? "N/A", filterCriteria);
        //                            break;
        //                        case "typeofacc":
        //                            query = ApplyStringFilter(query, emp => emp.TypeOfAcc ?? "N/A", filterCriteria);
        //                            break;
        //                        case "accountnumber":
        //                            query = ApplyStringFilter(query, emp => emp.AccountNumber ?? "N/A", filterCriteria);
        //                            break;
        //                            // يمكن إضافة المزيد من الخصائص هنا إذا لزم الأمر
        //                    }
        //                }
        //            }
        //        }

        //        // Apply sorting
        //        if (filterModel.MultiSortMeta != null && filterModel.MultiSortMeta.Any())
        //        {
        //            foreach (var sortMeta in filterModel.MultiSortMeta)
        //            {
        //                // جعل أسماء الخصائص غير حساسة لحالة الأحرف
        //                var propertyName = sortMeta.Field.ToLower();
        //                var propertyInfo = typeof(UnEmp).GetProperties()
        //                    .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

        //                if (propertyInfo != null)
        //                {
        //                    // Build the expression for ordering
        //                    var parameter = Expression.Parameter(typeof(UnEmp), "emp");
        //                    var propertyExpression = Expression.Property(parameter, propertyInfo);
        //                    var lambda = Expression.Lambda(propertyExpression, parameter);

        //                    // Build the correct method based on the sort direction
        //                    var method = sortMeta.Order == -1 ? "OrderByDescending" : "OrderBy";
        //                    var resultExpression = Expression.Call(
        //                        typeof(Queryable),
        //                        method,
        //                        new Type[] { typeof(UnEmp), propertyInfo.PropertyType },
        //                        query.Expression,
        //                        lambda
        //                    );

        //                    // Apply the sorting
        //                    query = query.Provider.CreateQuery<UnEmp>(resultExpression);
        //                }
        //            }
        //        }

        //        // Pagination - Use 5 rows if not specified
        //        var rowsToDisplay = filterModel.Rows > 0 ? filterModel.Rows : 5;

        //        var employeeDtos = await query
        //            .Skip(filterModel.First)
        //            .Take(rowsToDisplay)
        //            .Select(emp => new UNEmployeeDto
        //            {
        //                RefNo = emp.RefNo,
        //                EmpName = emp.EmpName,
        //                ArabicName = emp.ArabicName,
        //                MotherNameArabic = emp.MotherNameArabic,
        //                FatherNameArabic = emp.FatherNameArabic,
        //                IdNo = emp.IdNo,
        //                EmailAddress = emp.EmailAddress,
        //                MobileNo = emp.MobileNo,
        //                Gender = emp.Gender,
        //                BankName = emp.Bank.BanksName ?? "",
        //                TypeOfAcc = emp.TypeOfAcc ?? "",
        //                AccountNumber = emp.AccountNumber ?? "",
        //                Active = emp.Active
        //            })
        //            .AsNoTracking()
        //            .ToListAsync();

        //        return new ApiResponse<List<UNEmployeeDto>>("Employees retrieved successfully", employeeDtos, null, Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error
        //        // _logger.LogError($"Error retrieving employees: {ex.Message}");
        //        return new ApiResponse<List<UNEmployeeDto>>("Error occurred while retrieving employees", null, new List<string> { ex.Message });
        //    }
        //}

        //private IQueryable<UnEmp> ApplyEndLifeDateFilter(IQueryable<UnEmp> query, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (filterCriteria.Value is not DateOnly dateValue)
        //    {
        //        if (!DateOnly.TryParse(filterCriteria.Value?.ToString(), out dateValue))
        //            return query;
        //    }

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(emp => emp.EndLifeDate == dateValue);

        //        case "notequals":
        //            return query.Where(emp => emp.EndLifeDate != dateValue);

        //        case "before":
        //            return query.Where(emp => emp.EndLifeDate < dateValue);

        //        case "after":
        //            return query.Where(emp => emp.EndLifeDate > dateValue);

        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<UnEmp> ApplyStartLifeDateFilter(IQueryable<UnEmp> query, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (filterCriteria.Value is not DateOnly dateValue)
        //    {
        //        if (!DateOnly.TryParse(filterCriteria.Value?.ToString(), out dateValue))
        //            return query;
        //    }

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(emp => emp.StartLifeDate == dateValue);

        //        case "notequals":
        //            return query.Where(emp => emp.StartLifeDate != dateValue);

        //        case "before":
        //            return query.Where(emp => emp.StartLifeDate < dateValue);

        //        case "after":
        //            return query.Where(emp => emp.StartLifeDate > dateValue);

        //        default:
        //            return query;
        //    }
        //}

        //// Helper methods for filtering
        //private IQueryable<T> ApplyStringFilter<T>(IQueryable<T> query, Expression<Func<T, string>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    var value = filterCriteria.Value?.ToString(); // Convert dynamic value to string

        //    if (string.IsNullOrEmpty(value))
        //        return query;

        //    switch (matchMode)
        //    {
        //        case "startswith":
        //            return query.Where(Expression.Lambda<Func<T, bool>>(
        //                Expression.Call(
        //                    propertySelector.Body,
        //                    typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
        //                    Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "contains":
        //            return query.Where(Expression.Lambda<Func<T, bool>>(
        //                Expression.Call(
        //                    propertySelector.Body,
        //                    typeof(string).GetMethod("Contains", new[] { typeof(string) }),
        //                    Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "endswith":
        //            return query.Where(Expression.Lambda<Func<T, bool>>(
        //                Expression.Call(
        //                    propertySelector.Body,
        //                    typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
        //                    Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<T, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(value)),
        //                propertySelector.Parameters));
        //        case "notequals":
        //        case "notequal":
        //            return query.Where(Expression.Lambda<Func<T, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(value)),
        //                propertySelector.Parameters));
        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<UnEmp> ApplyNumericFilter(IQueryable<UnEmp> query, Expression<Func<UnEmp, int?>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (filterCriteria.Value is not int numericValue)
        //    {
        //        if (!int.TryParse(filterCriteria.Value?.ToString(), out numericValue))
        //            return query;
        //    }

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "notequals":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "lt":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.LessThan(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "lte":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.LessThanOrEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "gt":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.GreaterThan(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "gte":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.GreaterThanOrEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<UnEmp> ApplyDateFilter(IQueryable<UnEmp> query, Expression<Func<UnEmp, DateTime?>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (filterCriteria.Value is not DateTime dateValue)
        //    {
        //        if (!DateTime.TryParse(filterCriteria.Value?.ToString(), out dateValue))
        //            return query;
        //    }

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        case "notequals":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        case "before":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.LessThan(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        case "after":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.GreaterThan(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<UnEmp> ApplyBooleanFilter(IQueryable<UnEmp> query, Expression<Func<UnEmp, bool?>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (filterCriteria.Value is not bool boolValue)
        //    {
        //        if (!bool.TryParse(filterCriteria.Value?.ToString(), out boolValue))
        //            return query;
        //    }

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(boolValue)),
        //                propertySelector.Parameters));

        //        case "notequals":
        //            return query.Where(Expression.Lambda<Func<UnEmp, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(boolValue)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}
        public async Task<ApiResponse<List<UNEmployeeDto>>> GetAllEmployeesAsync(FilterModel filterRequest, string? PoNumber)
        {
            try
            {
                IQueryable<UnEmp> query;

                if (!string.IsNullOrEmpty(PoNumber))
                {
                    query = from emp in _context.UnEmps
                            join coo in _context.CooPos on emp.CooId equals coo.CooId
                            where coo.PoNum == PoNumber
                            select emp;
                }
                else
                {
                    query = _context.UnEmps.AsQueryable();
                }

                // Apply global filter
                if (!string.IsNullOrEmpty(filterRequest.GlobalFilter))
                {
                    query = query.Where(emp =>
                        emp.EmpName.Contains(filterRequest.GlobalFilter) ||
                        emp.ArabicName.Contains(filterRequest.GlobalFilter) ||
                        emp.EmailAddress.Contains(filterRequest.GlobalFilter) ||
                        emp.MobileNo.ToString().Contains(filterRequest.GlobalFilter) ||
                        emp.Gender.Contains(filterRequest.GlobalFilter));
                }

                // Apply individual filters dynamically
                query = FilterHelper.ApplyFilters(query, filterRequest);

                // Total count after filters
                var totalCount = await query.CountAsync();

                // Pagination
                var rowsToDisplay = filterRequest.Rows > 0 ? filterRequest.Rows : 5;

                // Fetch data and project to DTO
                var employeeListQuery = query.Select(emp => new UNEmployeeDto
                {
                    RefNo = emp.RefNo,
                    EmpName = emp.EmpName,
                    ArabicName = emp.ArabicName,
                    MotherNameArabic = emp.MotherNameArabic,
                    FatherNameArabic = emp.FatherNameArabic,
                    IdNo = emp.IdNo,
                    EmailAddress = emp.EmailAddress,
                    MobileNo = emp.MobileNo,
                    Gender = emp.Gender,
                    BankName = emp.Bank != null ? emp.Bank.BanksName : "",
                    IsDelegated = emp.IsDelegated,
                    TypeOfAcc = emp.TypeOfAcc ?? "",
                    AccountNumber = emp.AccountNumber ?? "",
                    Active = emp.Active,
                    OldEmployment = emp.OldEmployment,
                    SecurityCheck = emp.SecurityCheck,
                    cooNumber = emp.Coo != null ? emp.Coo.CooNumber : "",
                    PoNumber = (PoNumber != null && emp.Coo != null) ? emp.Coo.CooPos.FirstOrDefault(x => x.CooId == emp.Coo.CooId).PoNum : "",
                });
                // Fetch all data into memory
                var employeeList = await employeeListQuery     // A -> Z
                      .AsNoTracking()
                      .ToListAsync();


                // Additional filtering on DTO (PoNumber only)
                if (filterRequest.Filters != null && filterRequest.Filters.Any())
                {
                    foreach (var filter in filterRequest.Filters)
                    {
                        string field = filter.Key.ToLower();
                        string filterJsonString = System.Text.Json.JsonSerializer.Serialize(filter.Value);
                        JsonElement filterValue = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(filterJsonString);

                        if (filterValue.TryGetProperty("Value", out JsonElement valueElement) &&
                            valueElement.ValueKind == JsonValueKind.String &&
                            !string.IsNullOrEmpty(valueElement.GetString()) &&
                            filterValue.TryGetProperty("MatchMode", out JsonElement matchModeElement))
                        {
                            string matchMode = matchModeElement.GetString();
                            string value = valueElement.GetString().ToLower();

                            if (field == "ponumber")
                            {
                                switch (matchMode.ToLower())
                                {
                                    case "equals":
                                        employeeList = employeeList.Where(x => x.PoNumber != null && x.PoNumber.ToLower() == value).ToList();
                                        break;
                                    case "notequals":
                                    case "not_equals":
                                        employeeList = employeeList.Where(x => x.PoNumber != null && x.PoNumber.ToLower() != value).ToList();
                                        break;
                                    case "startswith":
                                        employeeList = employeeList.Where(x => x.PoNumber != null && x.PoNumber.ToLower().StartsWith(value)).ToList();
                                        break;
                                    case "endswith":
                                        employeeList = employeeList.Where(x => x.PoNumber != null && x.PoNumber.ToLower().EndsWith(value)).ToList();
                                        break;
                                    case "contains":
                                        employeeList = employeeList.Where(x => x.PoNumber != null && x.PoNumber.ToLower().Contains(value)).ToList();
                                        break;
                                    case "notcontains":
                                        employeeList = employeeList.Where(x => x.PoNumber != null && !x.PoNumber.ToLower().Contains(value)).ToList();
                                        break;
                                }
                            }
                            if (field == "coonumber")
                            {
                                switch (matchMode.ToLower())
                                {
                                    case "equals":
                                        employeeList = employeeList.Where(x => x.cooNumber != null && x.cooNumber.ToLower() == value).ToList();
                                        break;
                                    case "notequals":
                                    case "not_equals":
                                        employeeList = employeeList.Where(x => x.cooNumber != null && x.cooNumber.ToLower() != value).ToList();
                                        break;
                                    case "startswith":
                                        employeeList = employeeList.Where(x => x.cooNumber != null && x.cooNumber.ToLower().StartsWith(value)).ToList();
                                        break;
                                    case "endswith":
                                        employeeList = employeeList.Where(x => x.cooNumber != null && x.cooNumber.ToLower().EndsWith(value)).ToList();
                                        break;
                                    case "contains":
                                        employeeList = employeeList.Where(x => x.cooNumber != null && x.cooNumber.ToLower().Contains(value)).ToList();
                                        break;
                                    case "notcontains":
                                        employeeList = employeeList.Where(x => x.cooNumber != null && !x.cooNumber.ToLower().Contains(value)).ToList();
                                        break;
                                }
                            }
                        }
                    }
                }

                // Multi-Sorting
                if (filterRequest?.MultiSortMeta?.Any() == true)
                {
                    IOrderedQueryable<UNEmployeeDto> orderedQuery = null;

                    foreach (var sort in filterRequest.MultiSortMeta)
                    {
                        var field = sort.Field;
                        var sortOrder = sort.Order;

                        if (field.ToLower() == "ponumber")
                        {
                            PropertyInfo property = typeof(UNEmployeeDto).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (property == null) continue;

                            var parameter = Expression.Parameter(typeof(UNEmployeeDto), "entity");
                            var propertyExpression = Expression.Property(parameter, property);
                            var lambda = Expression.Lambda(propertyExpression, parameter);

                            var method = sortOrder == 1 ? "OrderBy" : "OrderByDescending";

                            var orderByExpression = Expression.Call(typeof(Queryable), method, new[] { typeof(UNEmployeeDto), property.PropertyType }, employeeList.AsQueryable().Expression, lambda);

                            orderedQuery = orderedQuery == null
                                ? (IOrderedQueryable<UNEmployeeDto>)employeeList.AsQueryable().Provider.CreateQuery<UNEmployeeDto>(orderByExpression)
                                : (IOrderedQueryable<UNEmployeeDto>)orderedQuery.Provider.CreateQuery<UNEmployeeDto>(orderByExpression);
                        }
                    }

                    if (orderedQuery != null)
                        employeeList = orderedQuery.ToList();
                }

                // Apply pagination at the end
                employeeList = employeeList
                    .Skip(filterRequest.First)
                    .Take(rowsToDisplay)
                    .ToList();

                return new ApiResponse<List<UNEmployeeDto>>("Employees retrieved successfully", employeeList, null, totalCount);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<UNEmployeeDto>>(
                    "Error occurred while retrieving employees",
                    null,
                    new List<string> { ex.Message, ex.StackTrace },
                    0
                );
            }
        }

        public async Task<ApiResponse<DisplyUNEmployeeDto>> GetByIdEmployeeAsync(int RefNo)
        {
            try
            {
                var employee = await _context.UnEmps
                    .Include(e => e.Bank)
                    .FirstOrDefaultAsync(x => x.RefNo == RefNo);

                if (employee == null)
                {
                    return new ApiResponse<DisplyUNEmployeeDto>("Employee not found", null, new List<string> { "Employee not found with the provided RefNo." });
                }

                var employeeDto = new DisplyUNEmployeeDto
                {
                    personal = new DisplyUNEmployeeDto.DisplyPersonal
                    {
                        RefNo = employee.RefNo,
                        EmpName = employee?.EmpName,
                        ArabicName = employee?.ArabicName,
                        MotherNameArabic = employee?.MotherNameArabic,
                        FatherNameArabic = employee?.FatherNameArabic,
                        IdNo = employee?.IdNo,
                        EmailAddress = employee?.EmailAddress,
                        MobileNo = employee?.MobileNo,
                        OldEmployment = employee?.OldEmployment,
                        SecurityCheck = employee?.SecurityCheck,
                        Gender = employee?.Gender?.ToLower() switch
                        {
                            "male" => 0,
                            "female" => 1,
                            _ => (int?)null
                        }
                    },

                    bankInfo = new DisplyUNEmployeeDto.DisplyBankInfo
                    {
                        BankId = employee?.BankId,
                        TypeOfAcc = employee?.TypeOfAcc,
                        AccountNumber = employee?.AccountNumber,
                        IsDelegated = employee.IsDelegated,
                    },
                };

                return new ApiResponse<DisplyUNEmployeeDto>("Employee retrieved successfully", employeeDto, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<DisplyUNEmployeeDto>("Error occurred while retrieving employee", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<FileResponseDto>>> UploadFilesAsync(int refNo, List<IFormFile> files)
        {
            try
            {
                var employee = await _context.UnEmps.FindAsync(refNo);
                if (employee == null)
                {
                    return new ApiResponse<IEnumerable<FileResponseDto>>("Employee not found", null, new List<string> { "The employee with the specified ID does not exist." });
                }

                if (files == null || !files.Any())
                {
                    return new ApiResponse<IEnumerable<FileResponseDto>>("No files uploaded", null, new List<string> { "Please upload at least one file." });
                }

                string contentRootPath = _configuration["FileStorage:FilesEmployeesPath"];
                var employeeFolderPath = Path.Combine(contentRootPath, employee.EmpName.ToString());

                if (!Directory.Exists(employeeFolderPath))
                {
                    Directory.CreateDirectory(employeeFolderPath);
                }

                var uploadedFiles = new List<FileResponseDto>();
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var originalName = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "_");
                        var extension = Path.GetExtension(file.FileName);

                        // التحقق إذا في ملف بنفس الاسم الأساسي داخل نفس المجلد (بدون النظر للتاريخ)
                        bool isDuplicate = Directory.GetFiles(employeeFolderPath)
                            .Select(f => Path.GetFileNameWithoutExtension(f).Split('_')[0]) // نأخذ الاسم الأساسي قبل التاريخ
                            .Any(f => string.Equals(f, originalName, StringComparison.OrdinalIgnoreCase));

                        if (isDuplicate)
                        {
                            return new ApiResponse<IEnumerable<FileResponseDto>>("Duplicate file name", null,
                                new List<string> { $"A file with the name '{originalName}' already exists for this employee." });
                        }

                        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                        var uniqueFileName = $"{originalName}_{timestamp}{extension}";
                        var filePath = Path.Combine(employeeFolderPath, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        uploadedFiles.Add(new FileResponseDto { FileName = uniqueFileName });

                        if (string.IsNullOrEmpty(employee.FolderPath))
                        {
                            employee.FolderPath = employee.EmpName.ToString();
                            _context.UnEmps.Update(employee);
                            await _context.SaveChangesAsync();
                        }
                    }
                }


                return new ApiResponse<IEnumerable<FileResponseDto>>("Files uploaded successfully", uploadedFiles, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<FileResponseDto>>("File upload failed", null, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<IEnumerable<FileResponseDto>>> GetEmployeeFilesAsync(int refNo)
        {
            try
            {
                var employee = await _context.UnEmps.FindAsync(refNo);
                if (employee == null)
                {
                    return new ApiResponse<IEnumerable<FileResponseDto>>("Employee not found", null, new List<string> { "The employee with the specified ID does not exist." });
                }

                string contentRootPath = _configuration["FileStorage:FilesEmployeesPath"];
                var employeeFolderPath = Path.Combine(contentRootPath, employee.EmpName.ToString());

                if (!Directory.Exists(employeeFolderPath))
                {
                    return new ApiResponse<IEnumerable<FileResponseDto>>("No files found", null, new List<string> { "The employee does not have any uploaded files." });
                }

                var files = Directory.GetFiles(employeeFolderPath)
                    .Select(file => new FileResponseDto { FileName = Path.GetFileName(file) })
                    .ToList();

                return new ApiResponse<IEnumerable<FileResponseDto>>("Files retrieved successfully", files, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<FileResponseDto>>("Failed to retrieve files", null, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<(byte[] content, string contentType, string fileName)>> DownloadFileAsync(int refNo, string fileName)
        {
            try
            {
                var employee = await _context.UnEmps.FindAsync(refNo);
                if (employee == null)
                {
                    return new ApiResponse<(byte[] content, string contentType, string fileName)>("Employee not found", (null, null, null), new List<string> { "The employee with the specified ID does not exist." });
                }

                string contentRootPath = _configuration["FileStorage:FilesEmployeesPath"];
                var employeeFolderPath = Path.Combine(contentRootPath, employee.EmpName.ToString());
                var filePath = Path.Combine(employeeFolderPath, fileName); // إضافة اسم الملف إلى المسار

                if (!File.Exists(filePath))
                {
                    return new ApiResponse<(byte[] content, string contentType, string fileName)>("File not found", (null, null, null), new List<string> { "The requested file does not exist." });
                }

                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = GetContentType(fileExtension);

                return new ApiResponse<(byte[] content, string contentType, string fileName)>("File downloaded successfully", (fileBytes, contentType, fileName), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<(byte[] content, string contentType, string fileName)>("File download failed", (null, null, null), new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<bool>> DeleteFileAsync(int refNo, string fileName)
        {
            try
            {
                var employee = await _context.UnEmps.FindAsync(refNo);
                if (employee == null)
                {
                    return new ApiResponse<bool>("Employee not found", false, new List<string> { "The employee with the specified ID does not exist." });
                }

                string contentRootPath = _configuration["FileStorage:FilesEmployeesPath"];
                var employeeFolderPath = Path.Combine(contentRootPath, employee.EmpName.ToString());
                var filePath = Path.Combine(employeeFolderPath, fileName); // إضافة اسم الملف إلى المسار

                if (!File.Exists(filePath))
                {
                    return new ApiResponse<bool>("File not found", false, new List<string> { "The requested file does not exist." });
                }

                try
                {
                    File.Delete(filePath);
                    return new ApiResponse<bool>("File deleted successfully", true, null);
                }
                catch (Exception ex)
                {
                    return new ApiResponse<bool>("File deletion failed", false, new List<string> { ex.Message });
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>("File deletion failed", false, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<bool>> DeleteAllFilesAsync(int refNo)
        {
            try
            {
                var employee = await _context.UnEmps.FindAsync(refNo);
                if (employee == null)
                {
                    return new ApiResponse<bool>("Employee not found", false, new List<string> { "The employee with the specified ID does not exist." });
                }

                string contentRootPath = _configuration["FileStorage:FilesEmployeesPath"];
                var employeeFolderPath = Path.Combine(contentRootPath, employee.EmpName.ToString());

                if (!Directory.Exists(employeeFolderPath))
                {
                    return new ApiResponse<bool>("No files found", false, new List<string> { "The employee does not have any uploaded files." });
                }

                try
                {
                    var files = Directory.GetFiles(employeeFolderPath);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                    return new ApiResponse<bool>("All files deleted successfully", true, null);
                }
                catch (Exception ex)
                {
                    return new ApiResponse<bool>("Failed to delete files", false, new List<string> { ex.Message });
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>("Failed to delete files", false, new List<string> { ex.Message });
            }
        }


        private string GetContentType(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                default:
                    return "application/octet-stream"; // Default binary type
            }
        }

    }
}
