using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using UNNew.DTOS.BankDtos;
using UNNew.DTOS.ContractDtos;
using UNNew.DTOS.DsaDto;
using UNNew.DTOS.InsuranceDtos;
using UNNew.DTOS.InvoiceDto;
using UNNew.DTOS.LaptopDto;
using UNNew.DTOS.SalaryDtos;
using UNNew.Filters;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class SalaryManagmentService : ISalaryManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SalaryManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        //public async Task<ApiResponse<List<SalaryDto>>> GetAllEmployeeSalaryAsync(FilterModel filterModel, int Id)
        //{
        //    try
        //    {
        //        // جلب جميع CooPos مسبقًا لتفادي استعلام N+1
        //        var cooPosList = await _context.CooPos.ToListAsync();

        //        // استعلام لجلب كل السجلات المرتبطة بـ RefNo معيّن
        //        var query = _context.SalaryTrans
        //            .Include(x => x.RefNoNavigation)
        //                .ThenInclude(x => x.Coo)
        //            .Include(x => x.Client)
        //            .Include(x => x.Team)
        //            .Where(x => x.RefNoNavigation.RefNo == Id)
        //            .AsQueryable();

        //        // Apply global filter
        //        if (!string.IsNullOrEmpty(filterModel.GlobalFilter))
        //        {
        //            var globalFilter = filterModel.GlobalFilter.ToLower();
        //            query = query.Where(employeeSalary =>
        //                (employeeSalary.RefNoNavigation != null && employeeSalary.RefNoNavigation.EmpName.ToLower().Contains(globalFilter)) ||
        //                (employeeSalary.Client != null && employeeSalary.Client.ClientName.ToLower().Contains(globalFilter)) ||
        //                (employeeSalary.Team != null && employeeSalary.Team.TeamName.ToLower().Contains(globalFilter)) ||
        //                employeeSalary.SlaryMonth.ToString().Contains(globalFilter));
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
        //                    filterCriteria.MatchMode = filterCriteria.MatchMode?.ToLower();

        //                    switch (propertyName.ToLower())
        //                    {
        //                        case "employeename":
        //                            query = ApplyStringFilter(query, x => x.RefNoNavigation.EmpName, filterCriteria);
        //                            break;
        //                        case "coo":
        //                            query = ApplyStringFilter(query, x => x.RefNoNavigation.Coo.CooNumber, filterCriteria);
        //                            var poNumber = cooPosList.FirstOrDefault(x => x.CooId == x.CooId);
        //                            var poNumValue = poNumber != null ? poNumber.PoNum : "";
        //                            query = ApplyStringFilter(query, x => poNumValue, filterCriteria);
        //                            break;
        //                        case "teamname":
        //                            query = ApplyStringFilter(query, x => x.Team.TeamName, filterCriteria);
        //                            break;
        //                        case "clientname":
        //                            query = ApplyStringFilter(query, x => x.Client.ClientName, filterCriteria);
        //                            break;
        //                        case "basicsalaryinusd":
        //                            query = ApplyNumericFilter(query, x => x.SalaryUsd, filterCriteria);
        //                            break;
        //                        case "totalsalarycalculatedinsyrianpounds":
        //                            query = ApplyNumericFilter(query, x => x.Ammount, filterCriteria);
        //                            break;
        //                        case "salarymonth":
        //                            query = ApplyNumericFilter(query, x => x.SlaryMonth, filterCriteria);
        //                            break;
        //                        case "salaryyear":
        //                            query = ApplyNumericFilter(query, x => x.SlaryYear, filterCriteria);
        //                            break;
        //                    }
        //                }
        //            }
        //        }

        //        // عدد النتائج بعد التصفية
        //        var count = await query.CountAsync();

        //        var employeeSalaries = await query.ToListAsync();

        //        if (!employeeSalaries.Any())
        //        {
        //            return new ApiResponse<List<SalaryDto>>("No active employees found", null, new List<string> { "No active employees found with valid contracts." });
        //        }

        //        var salaryDtos = employeeSalaries.Select(employeeSalary =>
        //        {
        //            var coo = employeeSalary.RefNoNavigation?.Coo;
        //            var cooId = coo?.CooId ?? 0;

        //            var poNumber = cooId != 0
        //                ? cooPosList.FirstOrDefault(x => x.CooId == cooId)?.PoNum ?? ""
        //                : "";

        //            return new SalaryDto
        //            {
        //                EmployeeName = employeeSalary.RefNoNavigation?.EmpName ?? "",
        //                TeamName = employeeSalary.Team?.TeamName ?? "",
        //                ClientName = employeeSalary.Client?.ClientName ?? "",
        //                BasicSalaryinUSD = employeeSalary.SalaryUsd,
        //                TotalSalaryCalculatedinSyrianPounds = employeeSalary.Ammount,
        //                SlaryMonth = employeeSalary.SlaryMonth,
        //                SlaryYear = employeeSalary.SlaryYear,
        //                CooNumber = coo?.CooNumber ?? "",
        //                PoNumber = poNumber
        //            };
        //        }).ToList();

        //        return new ApiResponse<List<SalaryDto>>("Employee Salary fetched successfully", salaryDtos, null, count);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse<List<SalaryDto>>("Error fetching Employee Salary", null, new List<string> { ex.Message });
        //    }
        //}


        //private IQueryable<SalaryTran> ApplyStringFilter(IQueryable<SalaryTran> query, Expression<Func<SalaryTran, string>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    var value = filterCriteria.Value;

        //    switch (matchMode)
        //    {
        //        case "startswith":
        //            return query.Where(Expression.Lambda<Func<SalaryTran, bool>>(
        //                Expression.Call(propertySelector.Body, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "contains":
        //            return query.Where(Expression.Lambda<Func<SalaryTran, bool>>(
        //                Expression.Call(propertySelector.Body, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "endswith":
        //            return query.Where(Expression.Lambda<Func<SalaryTran, bool>>(
        //                Expression.Call(propertySelector.Body, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<SalaryTran, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<SalaryTran> ApplyNumericFilter(IQueryable<SalaryTran> query, Expression<Func<SalaryTran, int?>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (!int.TryParse(filterCriteria.Value, out var numericValue))
        //        return query;

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<SalaryTran, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "notequals":
        //            return query.Where(Expression.Lambda<Func<SalaryTran, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "lt":
        //            return query.Where(Expression.Lambda<Func<SalaryTran, bool>>(
        //                Expression.LessThan(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "lte":
        //            return query.Where(Expression.Lambda<Func<SalaryTran, bool>>(
        //                Expression.LessThanOrEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "gt":
        //            return query.Where(Expression.Lambda<Func<SalaryTran, bool>>(
        //                Expression.GreaterThan(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "gte":
        //            return query.Where(Expression.Lambda<Func<SalaryTran, bool>>(
        //                Expression.GreaterThanOrEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}

        public async Task<ApiResponse<List<SalaryDto>>> GetAllEmployeeSalaryAsync(FilterModel filterModel, int Id,int? ContractId)
        {
            try
            {
                var cooPosList = await _context.CooPos.ToListAsync();
                var employeeCoosList = await _context.EmployeeCoos.ToListAsync(); // جلبهم مرة وحدة

                IQueryable<SalaryTran> query;
                if (ContractId == null)
                {
                     query = _context.SalaryTrans
                       .Include(x => x.RefNoNavigation)
                       .Include(x => x.Client)
                       .Include(x => x.Team)
                       .Include(x => x.EmployeeCoo)
                           .ThenInclude(x => x.Coo)
                       .Where(x => x.RefNoNavigation.RefNo == Id )     
                       .OrderByDescending(x => x.SlaryYear)
                       .ThenByDescending(x => x.SlaryMonth)
                       .AsQueryable();
                }
                else
                {
                     query = _context.SalaryTrans
                      .Include(x => x.RefNoNavigation)
                      .Include(x => x.Client)
                      .Include(x => x.Team)
                      .Include(x => x.EmployeeCoo)
                          .ThenInclude(x => x.Coo)
                      .Where(x => x.RefNoNavigation.RefNo == Id && x.ContractId==ContractId)
                      .OrderByDescending(x => x.SlaryYear)
                      .ThenByDescending(x => x.SlaryMonth)
                      .AsQueryable();
                }

                if (!string.IsNullOrEmpty(filterModel.GlobalFilter))
                {
                    var searchTerm = filterModel.GlobalFilter.ToLower();
                    query = query.Where(salary =>
                        (salary.RefNoNavigation != null && salary.RefNoNavigation.EmpName.ToLower().Contains(searchTerm)) ||
                        (salary.Team != null && salary.Team.TeamName.ToLower().Contains(searchTerm)) ||
                        (salary.Client != null && salary.Client.ClientName.ToLower().Contains(searchTerm)) ||
                        (salary.RefNoNavigation != null && salary.RefNoNavigation.Coo != null && salary.RefNoNavigation.Coo.CooNumber.ToLower().Contains(searchTerm)) ||
                        salary.SalaryUsd.ToString().Contains(searchTerm) ||
                        salary.Ammount.ToString().Contains(searchTerm) ||
                        salary.SlaryMonth.ToString().Contains(searchTerm) ||
                        salary.SlaryYear.ToString().Contains(searchTerm)
                    );
                }


                query = FilterHelper.ApplyFilters(query, filterModel);

                var count = await query.CountAsync();
                var employeeSalaries = await query.ToListAsync();

                if (!employeeSalaries.Any())
                {
                    return new ApiResponse<List<SalaryDto>>("No active employees found", null, new List<string> { "No active employees found with valid contracts." });
                }

                var salaryDtos = employeeSalaries.Select(employeeSalary =>
                {
                    var empId = employeeSalary.RefNoNavigation?.RefNo ?? 0;

                    return new SalaryDto
                    {
                        EmployeeName = employeeSalary.RefNoNavigation?.EmpName ?? "",
                        SalaryId = employeeSalary.TransId,
                        FatherNameArabic = employeeSalary.RefNoNavigation?.FatherNameArabic ?? "",
                        IsDelegated = employeeSalary.RefNoNavigation?.IsDelegated ?? false,
                        TeamName = employeeSalary.Team?.TeamName ?? "",
                        ClientName = employeeSalary.Client?.ClientName ?? "",
                        BasicSalaryinUSD = employeeSalary.SalaryUsd,
                        TotalSalaryCalculatedinSyrianPounds = employeeSalary.Ammount,
                        SlaryMonth = employeeSalary.SlaryMonth,
                        SlaryYear = employeeSalary.SlaryYear,
                        CooNumber = employeeSalary.EmployeeCoo?.Coo?.CooNumber ?? " ",
                        PoNumber = cooPosList.FirstOrDefault(x => x.CooId == employeeSalary.EmployeeCoo?.Coo?.CooId)?.PoNum ?? "",
                        ArabicName = employeeSalary.RefNoNavigation?.ArabicName ?? "",
                        BankName = employeeSalary.RefNoNavigation?.BankName ?? "",
                        AccountNumber = employeeSalary.RefNoNavigation?.AccountNumber ?? "",
                        ContractId = employeeCoosList.FirstOrDefault(x => x.EmpId == empId)?.Id ?? 0,
                        
                        
                    };
                }).ToList();


                // تطبيق الفلاتر على DTOs للحقول التي لا يمكن فلترتها في الاستعلام الأساسي
                if (filterModel.Filters != null && filterModel.Filters.Any())
                {
                    foreach (var filter in filterModel.Filters)
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

                            if (filterValue.TryGetProperty("MatchMode", out JsonElement matchModeElement))
                            {
                                string matchMode = matchModeElement.GetString();

                                if (filter.Key.ToLower() == "coonumber")
                                {
                                    string value = valueElement.GetString().ToLower();

                                    switch (matchMode.ToLower())
                                    {
                                        case "equals":
                                            salaryDtos = salaryDtos.Where(x => x.CooNumber != null && x.CooNumber.ToLower() == value).ToList();
                                            break;
                                        case "notequals":
                                        case "not_equals":
                                            salaryDtos = salaryDtos.Where(x => x.CooNumber != null && x.CooNumber.ToLower() != value).ToList();
                                            break;
                                        case "startswith":
                                            salaryDtos = salaryDtos.Where(x => x.CooNumber != null && x.CooNumber.ToLower().StartsWith(value)).ToList();
                                            break;
                                        case "endswith":
                                            salaryDtos = salaryDtos.Where(x => x.CooNumber != null && x.CooNumber.ToLower().EndsWith(value)).ToList();
                                            break;
                                        case "contains":
                                            salaryDtos = salaryDtos.Where(x => x.CooNumber != null && x.CooNumber.ToLower().Contains(value)).ToList();
                                            break;
                                        case "notcontains":
                                            salaryDtos = salaryDtos.Where(x => x.CooNumber != null && !x.CooNumber.ToLower().Contains(value)).ToList();
                                            break;
                                    }
                                }
                                else if (filter.Key.ToLower() == "ponumber")
                                {
                                    string value = valueElement.GetString().ToLower();

                                    switch (matchMode.ToLower())
                                    {
                                        case "equals":
                                            salaryDtos = salaryDtos.Where(x => x.PoNumber != null && x.PoNumber.ToLower() == value).ToList();
                                            break;
                                        case "notequals":
                                        case "not_equals":
                                            salaryDtos = salaryDtos.Where(x => x.PoNumber != null && x.PoNumber.ToLower() != value).ToList();
                                            break;
                                        case "startswith":
                                            salaryDtos = salaryDtos.Where(x => x.PoNumber != null && x.PoNumber.ToLower().StartsWith(value)).ToList();
                                            break;
                                        case "endswith":
                                            salaryDtos = salaryDtos.Where(x => x.PoNumber != null && x.PoNumber.ToLower().EndsWith(value)).ToList();
                                            break;
                                        case "contains":
                                            salaryDtos = salaryDtos.Where(x => x.PoNumber != null && x.PoNumber.ToLower().Contains(value)).ToList();
                                            break;
                                        case "notcontains":
                                            salaryDtos = salaryDtos.Where(x => x.PoNumber != null && !x.PoNumber.ToLower().Contains(value)).ToList();
                                            break;
                                    }
                                }
                                else if (filter.Key.ToLower() == "arabicname")
                                {
                                    string value = valueElement.GetString().ToLower();

                                    switch (matchMode.ToLower())
                                    {
                                        case "equals":
                                            salaryDtos = salaryDtos.Where(x => x.ArabicName != null && x.ArabicName.ToLower() == value).ToList();
                                            break;
                                        case "notequals":
                                        case "not_equals":
                                            salaryDtos = salaryDtos.Where(x => x.ArabicName != null && x.ArabicName.ToLower() != value).ToList();
                                            break;
                                        case "startswith":
                                            salaryDtos = salaryDtos.Where(x => x.ArabicName != null && x.ArabicName.ToLower().StartsWith(value)).ToList();
                                            break;
                                        case "endswith":
                                            salaryDtos = salaryDtos.Where(x => x.ArabicName != null && x.ArabicName.ToLower().EndsWith(value)).ToList();
                                            break;
                                        case "contains":
                                            salaryDtos = salaryDtos.Where(x => x.ArabicName != null && x.ArabicName.ToLower().Contains(value)).ToList();
                                            break;
                                        case "notcontains":
                                            salaryDtos = salaryDtos.Where(x => x.ArabicName != null && !x.ArabicName.ToLower().Contains(value)).ToList();
                                            break;
                                    }
                                }
                                else if (filter.Key.ToLower() == "employeename")
                                {
                                    string value = valueElement.GetString().ToLower();

                                    switch (matchMode.ToLower())
                                    {
                                        case "equals":
                                            salaryDtos = salaryDtos.Where(x => x.EmployeeName != null && x.EmployeeName.ToLower() == value).ToList();
                                            break;
                                        case "notequals":
                                        case "not_equals":
                                            salaryDtos = salaryDtos.Where(x => x.EmployeeName != null && x.EmployeeName.ToLower() != value).ToList();
                                            break;
                                        case "startswith":
                                            salaryDtos = salaryDtos.Where(x => x.EmployeeName != null && x.EmployeeName.ToLower().StartsWith(value)).ToList();
                                            break;
                                        case "endswith":
                                            salaryDtos = salaryDtos.Where(x => x.EmployeeName != null && x.EmployeeName.ToLower().EndsWith(value)).ToList();
                                            break;
                                        case "contains":
                                            salaryDtos = salaryDtos.Where(x => x.EmployeeName != null && x.EmployeeName.ToLower().Contains(value)).ToList();
                                            break;
                                        case "notcontains":
                                            salaryDtos = salaryDtos.Where(x => x.EmployeeName != null && !x.EmployeeName.ToLower().Contains(value)).ToList();
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                // تطبيق الترتيب
                if (filterModel?.MultiSortMeta?.Any() == true)
                {
                    IOrderedQueryable<SalaryDto> orderedQuery = null;

                    foreach (var sort in filterModel.MultiSortMeta)
                    {
                        var field = sort.Field;
                        var sortOrder = sort.Order;

                        if (field.ToLower() == "coonumber" || field.ToLower() == "ponumber" || field.ToLower() == "arabicname" || field.ToLower() == "employeename")
                        {
                            PropertyInfo property = typeof(SalaryDto).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (property == null) continue;

                            var parameter = Expression.Parameter(typeof(SalaryDto), "entity");
                            var propertyExpression = Expression.Property(parameter, property);
                            var lambda = Expression.Lambda(propertyExpression, parameter);

                            var method = sortOrder == 1 ? "OrderBy" : "OrderByDescending";

                            var orderByExpression = Expression.Call(typeof(Queryable), method, new[] { typeof(SalaryDto), property.PropertyType }, salaryDtos.AsQueryable().Expression, lambda);

                            orderedQuery = orderedQuery == null
                                ? (IOrderedQueryable<SalaryDto>)salaryDtos.AsQueryable().Provider.CreateQuery<SalaryDto>(orderByExpression)
                                : (IOrderedQueryable<SalaryDto>)orderedQuery.Provider.CreateQuery<SalaryDto>(orderByExpression);
                        }
                    }

                    salaryDtos = orderedQuery?.ToList() ?? salaryDtos;
                }

                // تطبيق التقسيم إلى صفحات
                salaryDtos = salaryDtos
                    .Skip(filterModel.First)
                    .Take(filterModel.Rows > 0 ? filterModel.Rows : 5)
                    .ToList();

                return new ApiResponse<List<SalaryDto>>("Employee Salary fetched successfully", salaryDtos, null, count);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<SalaryDto>>("Error fetching Employee Salary", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<GetByIdSalaryDto>> GetEmployeeSalaryByIdAsync(int id)
        {
            try
            {
                // تحديد التاريخ الحالي بدون وقت
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

                // جلب السجل مع التضمينات اللازمة
                var employee = await _context.SalaryTrans
                    .Include(x => x.Team)
                    .Include(x => x.Client)
                    .Include(x => x.RefNoNavigation)
                    .Include(x => x.EmployeeCoo)
                        .ThenInclude(x => x.Coo)
                    .FirstOrDefaultAsync(x => x.TransId == id);

                if (employee == null)
                {
                    return new ApiResponse<GetByIdSalaryDto>(
                        "Employee not found",
                        null,
                        new List<string> { "No employee found with the provided ID." }
                    );
                }

                // جلب رقم الـ PO باستخدام CooId إن وجد
                var cooId = employee.EmployeeCoo?.Coo?.CooId;
                var poNumber = cooId.HasValue
                    ? _context.CooPos.FirstOrDefault(x => x.CooId == cooId.Value)?.PoNum ?? ""
                    : "";

                // بناء DTO للبيانات
                var updateSalaryDto = new GetByIdSalaryDto
                {
                    EmployeeName = employee.RefNoNavigation?.EmpName ?? "",
                    TeamName = employee.Team?.TeamName ?? "",
                    ClientName = employee.Client?.ClientName ?? "",
                    CooNumber = employee.EmployeeCoo?.Coo?.CooNumber ?? "",
                    PoNumber = poNumber,
                    BasicSalaryinUSD = employee.SalaryUsd,
                    TotalSalaryCalculatedinSyrianPounds = employee.Ammount ?? 0,
                    SickLeave = employee.SickLeave ?? 0,
                    DaysOff = employee.DaysOff ?? 0,
                    DownPayment = employee.DownPayment ?? 0,
                    Transportation = employee.Transportation ?? 0,
                    Mobile = employee.Mobile ?? 0,
                    Dsa = employee.Dsa ?? 0,
                    OverTimeWages = employee.OverTimeWages ?? 0,
                    NetSalary = employee.NetSalary ?? 0,
                    Laptop = employee.Laptop ?? 0,
                    TimeSheet = employee.TimeSheet,
                    DaysOn = employee.DaysOn,
                    SlaryMonth=employee.SlaryMonth,
                    SlaryYear=employee.SlaryYear,
                    LaptopRent=employee.LaptopRent,
                    Deductions=employee.Deductions,
                    Bonuses=employee.Bonuses
                };

                return new ApiResponse<GetByIdSalaryDto>(
                    "Employee Salary type fetched successfully",
                    updateSalaryDto,
                    null
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetByIdSalaryDto>(
                    "Error fetching Employee Salary",
                    null,
                    new List<string> { ex.Message, ex.InnerException?.Message ?? "No inner exception" }
                );
            }
        }


        public async Task<ApiResponse<List<GetAllDsaDto>>> GetAllDsaByEmployeddId(int id,int? ContractId, FilterModel filterModel)
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
                var cooPosList = await _context.CooPos.ToListAsync();
                List<SalaryTran> employees;

                if (ContractId == null)
                {
                    employees = await _context.SalaryTrans
                        .Include(x => x.RefNoNavigation)
                        .Include(x => x.Team)
                        .Include(x => x.Client)
                        .Include(x => x.EmployeeCoo)
                        .Where(x => x.RefNoNavigation.RefNo == id && x.Dsa != null)
                        .OrderByDescending(x => x.SlaryYear)
                        .ThenByDescending(x => x.SlaryMonth)
                        .ToListAsync();
                }
                else
                {
                    employees = await _context.SalaryTrans
                        .Include(x => x.RefNoNavigation)
                        .Include(x => x.Team)
                        .Include(x => x.Client)
                        .Include(x => x.EmployeeCoo)
                        .Where(x => x.RefNoNavigation.RefNo == id && x.ContractId == ContractId && x.Dsa != null)
                        .OrderByDescending(x => x.SlaryYear)
                        .ThenByDescending(x => x.SlaryMonth)
                        .ToListAsync();
                }

                if (employees == null || !employees.Any())
                {
                    return new ApiResponse<List<GetAllDsaDto>>(
                        "No salary records found for the specified employee.",
                        null,
                        new List<string> { "No salary has been calculated for the specified employee." }
                    );
                }

                // قائمة النتائج
                List<GetAllDsaDto> dsaList = new();

                foreach (var emp in employees)
                {
                    var cooId = emp?.EmployeeCoo?.CooId ?? 0;
                    var coonumber = cooId != 0
                        ? (await _context.Coos.FirstOrDefaultAsync(x => x.CooId == cooId))?.CooNumber
                        : null;

                    var poNum = cooId != 0
                        ? cooPosList.FirstOrDefault(x => x.CooId == cooId)?.PoNum
                        : null;

                    dsaList.Add(new GetAllDsaDto
                    {
                        Id=emp?.TransId,
                        EmployeeName = emp?.RefNoNavigation?.EmpName,
                        ArabicName = emp?.RefNoNavigation?.ArabicName,
                        FatherNameArabic = emp?.RefNoNavigation?.FatherNameArabic,
                        CooNumber = coonumber,
                        PoNumber = poNum,
                        Month = emp?.SlaryMonth,
                        Year = emp?.SlaryYear,
                        DsaValue = emp?.Dsa,
                        ClientName = emp?.Client?.ClientName,
                        TeamName = emp?.Team?.TeamName,
                        ContractId = emp?.ContractId ?? 0,
                        Tittle = emp?.EmployeeCoo?.Tittle,
                    });
                }


                if (filterModel.Filters != null && filterModel.Filters.Any())
                {
                    // فلترة بيانات CooNumber و PoNumber
                    foreach (var filter in filterModel?.Filters)
                    {
                        string field = filter.Key;
                        string filterJsonString = JsonSerializer.Serialize(filter.Value);
                        JsonElement filterValue = JsonSerializer.Deserialize<JsonElement>(filterJsonString);

                        if (!filterValue.TryGetProperty("Value", out JsonElement valueElement) ||
                            valueElement.ValueKind == JsonValueKind.Null ||
                            (valueElement.ValueKind == JsonValueKind.String && string.IsNullOrEmpty(valueElement.GetString())))
                        {
                            continue;
                        }

                        if (!filterValue.TryGetProperty("MatchMode", out JsonElement matchModeElement)) continue;

                        string matchMode = matchModeElement.GetString().ToLowerInvariant();

                        var property = typeof(GetAllDsaDto).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (property == null) continue;

                        var propertyType = property.PropertyType;
                        var parameter = Expression.Parameter(typeof(GetAllDsaDto), "x");
                        var propertyExpression = Expression.Property(parameter, property);

                        Expression filterExpression = null;

                        if (propertyType == typeof(string))
                        {
                            string stringValue = valueElement.ValueKind switch
                            {
                                JsonValueKind.String => valueElement.GetString()?.ToLowerInvariant(),
                                JsonValueKind.Number => valueElement.ToString()?.ToLowerInvariant(),
                                _ => null
                            };

                            if (stringValue == null) continue;

                            switch (matchMode)
                            {
                                case "equals":
                                    filterExpression = Expression.Equal(
                                        Expression.Call(propertyExpression, typeof(string).GetMethod("ToLower", Type.EmptyTypes)),
                                        Expression.Constant(stringValue, typeof(string))
                                    );
                                    break;

                                case "notequals":
                                case "not_equals":
                                    filterExpression = Expression.Not(Expression.Equal(
                                        Expression.Call(propertyExpression, typeof(string).GetMethod("ToLower", Type.EmptyTypes)),
                                        Expression.Constant(stringValue, typeof(string))
                                    ));
                                    break;

                                case "contains":
                                case "notcontains":
                                case "not_contains":
                                case "startswith":
                                case "endswith":
                                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                                    MethodInfo stringMethod = null;
                                    switch (matchMode)
                                    {
                                        case "contains":
                                            stringMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                            filterExpression = Expression.Call(
                                                Expression.Call(propertyExpression, toLowerMethod),
                                                stringMethod,
                                                Expression.Constant(stringValue)
                                            );
                                            break;

                                        case "notcontains":
                                        case "not_contains":
                                            stringMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                            filterExpression = Expression.Not(
                                                Expression.Call(
                                                    Expression.Call(propertyExpression, toLowerMethod),
                                                    stringMethod,
                                                    Expression.Constant(stringValue)
                                                )
                                            );
                                            break;

                                        case "startswith":
                                            stringMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                                            filterExpression = Expression.Call(
                                                Expression.Call(propertyExpression, toLowerMethod),
                                                stringMethod,
                                                Expression.Constant(stringValue)
                                            );
                                            break;

                                        case "endswith":
                                            stringMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                                            filterExpression = Expression.Call(
                                                Expression.Call(propertyExpression, toLowerMethod),
                                                stringMethod,
                                                Expression.Constant(stringValue)
                                            );
                                            break;
                                    }
                                    break;
                            }
                        }
                        else if (propertyType == typeof(int) || propertyType == typeof(int?))
                        {
                            int? intValue = valueElement.ValueKind switch
                            {
                                JsonValueKind.Number => valueElement.GetInt32(),
                                JsonValueKind.String when int.TryParse(valueElement.GetString(), out int parsed) => parsed,
                                _ => null
                            };

                            if (intValue == null) continue;

                            switch (matchMode)
                            {
                                case "equals":
                                    filterExpression = Expression.Equal(propertyExpression, Expression.Constant(intValue, propertyType));
                                    break;

                                case "notequals":
                                case "not_equals":
                                    filterExpression = Expression.NotEqual(propertyExpression, Expression.Constant(intValue, propertyType));
                                    break;

                                case "lessthan":
                                case "lt":
                                    filterExpression = Expression.LessThan(propertyExpression, Expression.Constant(intValue, propertyType));
                                    break;

                                case "lessthanorequalto":
                                case "lte":
                                    filterExpression = Expression.LessThanOrEqual(propertyExpression, Expression.Constant(intValue, propertyType));
                                    break;

                                case "greaterthan":
                                case "gt":
                                    filterExpression = Expression.GreaterThan(propertyExpression, Expression.Constant(intValue, propertyType));
                                    break;

                                case "greaterthanorequalto":
                                case "gte":
                                    filterExpression = Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(intValue, propertyType));
                                    break;
                            }
                        }

                        if (filterExpression != null)
                        {
                            var lambda = Expression.Lambda<Func<GetAllDsaDto, bool>>(filterExpression, parameter);
                            dsaList = dsaList.AsQueryable().Where(lambda).ToList();
                        }
                    }

                }


                // ترتيب البيانات حسب MultiSortMeta
                if (filterModel?.MultiSortMeta?.Any() == true)
                {
                    IOrderedQueryable<GetAllDsaDto> orderedQuery = null;

                    foreach (var sort in filterModel.MultiSortMeta)
                    {
                        var field = sort.Field;
                        var sortOrder = sort.Order;

                        // جلب الخاصية بناءً على اسم الحقل
                        PropertyInfo property = typeof(GetAllDsaDto).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (property == null) continue;

                        // إعداد المعاملات الخاصة بالـ LINQ
                        var parameter = Expression.Parameter(typeof(GetAllDsaDto), "entity");
                        var propertyExpression = Expression.Property(parameter, property);

                        // تحديد نوع الترتيب (تصاعدي أو تنازلي)
                        var method = sortOrder == 1 ? "OrderBy" : "OrderByDescending";

                        // إنشاء تعبير lambda بناءً على نوع البيانات للخاصية
                        var lambda = Expression.Lambda(propertyExpression, parameter);

                        // إنشاء استدعاء للـ Expression الخاص بـ LINQ
                        var orderByExpression = Expression.Call(typeof(Queryable), method, new[] { typeof(GetAllDsaDto), property.PropertyType }, dsaList.AsQueryable().Expression, lambda);

                        // تطبيق الترتيب على الكائن orderedQuery
                        orderedQuery = orderedQuery == null
                            ? (IOrderedQueryable<GetAllDsaDto>)dsaList.AsQueryable().Provider.CreateQuery<GetAllDsaDto>(orderByExpression)
                            : (IOrderedQueryable<GetAllDsaDto>)orderedQuery.Provider.CreateQuery<GetAllDsaDto>(orderByExpression);
                    }

                    // تطبيق الترتيب النهائي على القائمة
                    dsaList = orderedQuery?.ToList() ?? dsaList;
                }


                // تقسيم النتائج إلى صفحات
                var count = dsaList.Count;

                dsaList = dsaList
                    .Skip(filterModel.First)
                    .Take(filterModel.Rows > 0 ? filterModel.Rows : 5)
                    .ToList();

                return new ApiResponse<List<GetAllDsaDto>>("DSA fetched successfully", dsaList, null, count);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetAllDsaDto>>("Error fetching DSA", null, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<string>> AddDsa(AddDsaDto addDsaDto)
        {
            try
            {
                // تحقق من الشهر
                if (addDsaDto.Month < 1 || addDsaDto.Month > 12)
                {
                    return new ApiResponse<string>("Invalid month", null, new List<string> { "The month must be between 1 and 12." });
                }

                // تحقق من السنة
                if (addDsaDto.Year > DateTime.Now.Year)
                {
                    return new ApiResponse<string>("Invalid year", null, new List<string> { "The year cannot be in the future." });
                }

                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                // تحقق من وجود الموظف وعلاقته بالفريق والعميل
                var employee = await _context.EmployeeCoos
                    .Include(x => x.Emp)
                    .Include(x => x.Team)
                    .Include(x => x.Client)
                    .Include(x => x.LaptopNavigation)
                    .FirstOrDefaultAsync(x =>
                        x.Id == addDsaDto.ContractId &&
                        x.Emp != null &&
                        x.Emp.RefNo == addDsaDto.EmployeeId
                    );

                if (employee == null)
                {
                    return new ApiResponse<string>(
                        "Employee is not found in this contract or contract is not found",
                        null,
                        new List<string> { "The employee is either inactive, not assigned to this team/client, or does not exist." }
                    );
                }

                var salaryTran = new SalaryTran
                {
                    ContractId = addDsaDto.ContractId,
                    TransDate = DateTime.Now,
                    Dsa = addDsaDto.DsaValue,
                    RefNo = addDsaDto.EmployeeId,
                    TeamId = employee.TeamId,
                    ClientId = employee.ClientId,
                    SlaryMonth = addDsaDto.Month,
                    SlaryYear = addDsaDto.Year
                };

                // إضافة سجل SalaryTran
                await _context.SalaryTrans.AddAsync(salaryTran);
                await _context.SaveChangesAsync(); 

                var salaryEmployeeCoo = new SalaryEmployeeCoo
                {
                    EmployeeCooId = addDsaDto.ContractId,
                    SalaryId = salaryTran.TransId 
                };

                await _context.SalaryEmployeeCoos.AddAsync(salaryEmployeeCoo);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("DSA value added to salary successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    "An error occurred while updating the salary",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }


        public async Task<ApiResponse<string>> CalculateSalary(UpdateSalaryDto updateSalaryDto)
        {
            try
            {
           
                if (updateSalaryDto.Month < 1 || updateSalaryDto.Month > 12)
                {
                    return new ApiResponse<string>("Invalid month", null, new List<string> { "The month must be between 1 and 12." });
                }

                if (updateSalaryDto.Year > DateTime.Now.Year)
                {
                    return new ApiResponse<string>("Invalid year", null, new List<string> { "The year cannot be in the future." });
                }

                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
                var targetDate = new DateTime(updateSalaryDto.Year, updateSalaryDto.Month, 1);

                var employee = await _context.EmployeeCoos
                    .Include(x => x.Emp)
                    .Include(x => x.Team)
                    .Include(x => x.Client)
                    .Include(x => x.Coo)
                    .Include(x => x.LaptopNavigation)
                    .FirstOrDefaultAsync(x => x.EmpId == updateSalaryDto.EmployeeId && x.Id == updateSalaryDto.ContractId);

                if (employee == null)
                {
                    return new ApiResponse<string>("Employee is not active", null, new List<string> { "The employee is either inactive or does not exist." });
                }

                if (employee?.Coo?.CooDate != null)
                {
                    var contractYear = employee.Coo.CooDate.Value.Year;
                    if (updateSalaryDto.Year < contractYear)
                    {
                        return new ApiResponse<string>(
                            "Invalid year",
                            null,
                            new List<string> {
                        $"The entered year ({updateSalaryDto.Year}) is earlier than the contract signing year ({contractYear}), and data cannot be entered before the contract date."
                            }
                        );
                    }
                }

                var existingSalary = await _context.SalaryTrans
                    .FirstOrDefaultAsync(x => x.RefNo == updateSalaryDto.EmployeeId &&
                        x.ContractId == updateSalaryDto.ContractId &&
                        x.SlaryMonth == updateSalaryDto.Month &&
                        x.SlaryYear == updateSalaryDto.Year);

                var unMonthLeave = await _context.UnMonthLeave
                    .Where(x => x.YearNum != null && x.MonthNum != null && x.UnMonthLeave1 != null && x.ClientId== employee.ClientId)
                    .ToListAsync();

                var bestLeave = unMonthLeave
                    .Where(x => new DateTime(x.YearNum.Value, x.MonthNum.Value, 1) <= targetDate)
                    .OrderByDescending(x => new DateTime(x.YearNum.Value, x.MonthNum.Value, 1))
                    .ThenByDescending(x => x.CreatedAt)
                    .FirstOrDefault();

                decimal allowedLeave = (decimal)(bestLeave?.UnMonthLeave1 ?? 0f);

                decimal daysOffTaken = updateSalaryDto.DaysOff ?? 0;
                decimal excessLeave = Math.Max(0, Math.Floor(daysOffTaken - allowedLeave));
                int day = 30;

                if (employee.StartCont.HasValue && employee.EndCont.HasValue)
                {
                    if (employee.StartCont.Value.Month == updateSalaryDto.Month &&
                        employee.StartCont.Value.Year == updateSalaryDto.Year &&
                        employee.EndCont.Value.Month == updateSalaryDto.Month &&
                        employee.EndCont.Value.Year == updateSalaryDto.Year)
                    {
                        // العقد بدأ وانتهى بنفس الشهر
                        day = (employee.EndCont.Value.Day - employee.StartCont.Value.Day) + 1;
                    }
                    else
                    {
                        if (employee.StartCont.Value.Month == updateSalaryDto.Month &&
                            employee.StartCont.Value.Year == updateSalaryDto.Year)
                        {
                            day = 30 - employee.StartCont.Value.Day + 1;
                        }

                        if (employee.EndCont.Value.Month == updateSalaryDto.Month &&
                            employee.EndCont.Value.Year == updateSalaryDto.Year)
                        {
                            day = employee.EndCont.Value.Day;
                        }
                    }
                }



                day = day - (int)excessLeave;
               int allday = 30-day;
             
                decimal? daysOn = day;
                decimal phoneAllowance = 0;
                if (employee.IsMobile == true)
                {
                    var mobileComp = await _context.UnMobileCompensation
                        .Where(x => x.YearNum != null && x.MonthNum != null && x.UnMobileCompensation1 != null && x.ClientId== employee.ClientId)
                        .ToListAsync();

                    var bestMobile = mobileComp
                        .Where(x => x.YearNum.HasValue && x.MonthNum.HasValue &&
                                    x.MonthNum.Value >= 1 && x.MonthNum.Value <= 12 &&
                                    x.YearNum.Value > 0)
                        .Where(x => new DateTime(x.YearNum.Value, x.MonthNum.Value, 1) <= targetDate)
                        .OrderByDescending(x => new DateTime(x.YearNum.Value, x.MonthNum.Value, 1))
                        .ThenByDescending(x => x.CreatedAt)
                        .FirstOrDefault();

                    phoneAllowance = bestMobile?.UnMobileCompensation1 ?? 0;
                }

                decimal transportationAllowance = 0;
                if (employee.Transportation == true)
                {
                    var transportComp = await _context.UnTransportCompensation
                        .Where(x => x.YearNum != null && x.MonthNum != null && x.UnTransportCompensation1 != null && x.ClientId== employee.ClientId)
                        .ToListAsync();

                    var bestTransport = transportComp
                        .Where(x => new DateTime(x.YearNum.Value, x.MonthNum.Value, 1) <= targetDate)
                        .OrderByDescending(x => new DateTime(x.YearNum.Value, x.MonthNum.Value, 1))
                        .ThenByDescending(x => x.CreatedAt)
                        .FirstOrDefault();

                    transportationAllowance = bestTransport?.UnTransportCompensation1 ?? 0;
                    transportationAllowance = (transportationAllowance / 30m) * day;

                }

                decimal laptopAllowance = 0;
                decimal laptopRent = 0;
                if (!string.IsNullOrWhiteSpace(employee.LaptopNavigation?.Name))
                {
                    if (employee.LaptopNavigation.Activite == Activite.ICI)
                    {
                        var laptopRents = await _context.LaptopRents
                        .Where(x => x.LaptopType == employee.Laptop && x.Price > 0)
                        .ToListAsync();

                        var bestLaptopRent = laptopRents
                            .Where(x => new DateTime(x.Year, x.Month, 1) <= targetDate)
                            .OrderByDescending(x => new DateTime(x.Year, x.Month, 1))
                            .ThenByDescending(x => x.CreatedAt)
                            .FirstOrDefault();

                        laptopRent = bestLaptopRent?.Price ?? 0;

                    }
                    if (employee.LaptopNavigation.Activite == Activite.Employee)
                    {
                        var laptopRents = await _context.LaptopRents
                        .Where(x => x.LaptopType == employee.Laptop && x.Price > 0)
                        .ToListAsync();

                        var bestLaptopRent = laptopRents
                            .Where(x => new DateTime(x.Year, x.Month, 1) <= targetDate)
                            .OrderByDescending(x => new DateTime(x.Year, x.Month, 1))
                            .ThenByDescending(x => x.CreatedAt)
                            .FirstOrDefault();

                        laptopAllowance = bestLaptopRent?.Price ?? 0;

                    }
                  
                }

                decimal overtimeAllowance = (decimal)(updateSalaryDto.OverTimeWages ?? 0);
                decimal downPayment = updateSalaryDto.DownPayment ?? 0;

                var exchangeRates = await _context.UnRates
                    .Where(x => x.YearNum == targetDate.Year && x.MonthNum == targetDate.Month)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                decimal? exchangeRate = exchangeRates.FirstOrDefault()?.UnRate1;

                if (exchangeRate == null || exchangeRate == 0)
                {
                    exchangeRate = _context.UnRates
                        .Where(x => x.YearNum != null && x.MonthNum != null)
                        .AsEnumerable()
                        .Where(x => new DateTime(x.YearNum.Value, x.MonthNum.Value, 1) < targetDate && x.ClientId== employee.ClientId)
                        .OrderByDescending(x => new DateTime(x.YearNum.Value, x.MonthNum.Value, 1))
                        .ThenByDescending(x => x.CreatedAt)
                        .Select(x => (decimal?)x.UnRate1)
                        .FirstOrDefault();
                }

                if (exchangeRate == null || exchangeRate == 0 || employee.Salary == 0)
                {
                    return new ApiResponse<string>("Cannot calculate salary due to missing or zero exchange rate or salary.", null,
                        new List<string> { "Check salary and exchange rate values." });
                }

                decimal? basicSalary = employee.Salary * exchangeRate;
                decimal? daysOffDeduction = basicSalary.HasValue ? (basicSalary / 30m) * (allday) : 0;
                decimal? netSalary = basicSalary - daysOffDeduction;

                decimal? totalBeforeExchange = basicSalary - daysOffDeduction - downPayment +
                    phoneAllowance + transportationAllowance + laptopAllowance + overtimeAllowance;
                decimal? Amount = totalBeforeExchange;

                decimal? Deductions = daysOffDeduction + downPayment;
                decimal? Bonuses = phoneAllowance + transportationAllowance + laptopAllowance + overtimeAllowance;
              

                if (existingSalary != null)
                {
                    existingSalary.SickLeave = updateSalaryDto.SickLeave;
                    existingSalary.DaysOff = updateSalaryDto.DaysOff;
                    existingSalary.OverTimeWages = (double?)overtimeAllowance;
                    existingSalary.DownPayment = (int?)downPayment;
                    existingSalary.Ammount = (int?)Amount;
                    existingSalary.NetSalary = (int?)netSalary;
                    existingSalary.DaysOn = (int?)daysOn;
                    existingSalary.TransDate = DateTime.UtcNow;
                    existingSalary.ContractId = updateSalaryDto.ContractId;
                    existingSalary.SlaryMonth = updateSalaryDto.Month;
                    existingSalary.SlaryYear = updateSalaryDto.Year;
                    existingSalary.Mobile = (int?)phoneAllowance;
                    existingSalary.Laptop = (int?)laptopAllowance;
                    existingSalary.Transportation = (int?)transportationAllowance;
                    existingSalary.UnRate = (int?)exchangeRate;
                    existingSalary.LaptopRent = (int?)laptopRent;
                    existingSalary.TeamId = employee.TeamId;
                    existingSalary.ClientId = employee.ClientId;
                    existingSalary.Bonuses =(int?) Bonuses;
                    existingSalary.Deductions =(int?)Deductions;
                    existingSalary.Dsa =null;

       


                }
                else
                {
                    var salaryTran = new SalaryTran
                    {
                        RefNo = updateSalaryDto.EmployeeId,
                        SickLeave = updateSalaryDto.SickLeave,
                        DaysOff = updateSalaryDto.DaysOff,
                        OverTimeWages = (double?)overtimeAllowance,
                        DownPayment = (int?)downPayment,
                        Ammount = (int?)Amount,
                        DaysOn = (int?)daysOn,
                        TransDate = DateTime.UtcNow,
                        ContractId = updateSalaryDto.ContractId,
                        SalaryUsd = employee.Salary,
                        NetSalary = (int?)netSalary,
                        SlaryMonth = updateSalaryDto.Month,
                        SlaryYear = updateSalaryDto.Year,
                        Mobile = (int?)phoneAllowance,
                        Laptop = (int?)laptopAllowance,
                       Transportation = (int?)transportationAllowance,
                       UnRate= (int?)exchangeRate,
                       LaptopRent=(int?)laptopRent,
                       TeamId=employee.TeamId,
                       ClientId=employee.ClientId,
                       Bonuses = (int?)Bonuses,
                       Deductions = (int?)Deductions,
                       Dsa = null,
                    };

                    await _context.SalaryTrans.AddAsync(salaryTran);
                    await _context.SaveChangesAsync();

                    var salaryEmployee = new SalaryEmployeeCoo
                    {
                        EmployeeCooId = employee.Id,
                        SalaryId = salaryTran.TransId
                    };
                    await _context.SalaryEmployeeCoos.AddAsync(salaryEmployee);
                }

                await _context.SaveChangesAsync();

                return new ApiResponse<string>($"Salary updated successfully. Net Salary: {netSalary:F2}", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while updating the salary", null, new List<string> { ex.Message });
            }
        }



        //public async Task<ApiResponse<string>> CalculateSalary(UpdateSalaryDto updateSalaryDto)
        //{
        //    try
        //    {
        //        if (updateSalaryDto.Month < 1 || updateSalaryDto.Month > 12)
        //        {
        //            return new ApiResponse<string>("Invalid month", null, new List<string> { "The month must be between 1 and 12." });
        //        }

        //        if (updateSalaryDto.Year > DateTime.Now.Year)
        //        {
        //            return new ApiResponse<string>("Invalid year", null, new List<string> { "The year cannot be in the future." });
        //        }

        //        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        //        var employee = await _context.EmployeeCoos
        //            .Include(x => x.Emp)
        //            .Include(x => x.Team)
        //            .Include(x => x.Client)
        //            .Include(x => x.LaptopNavigation)
        //            .FirstOrDefaultAsync(x => x.EmpId == updateSalaryDto.EmployeeId && x.EndCont >= today
        //                && x.Emp.Active);

        //        if (employee == null)
        //        {
        //            return new ApiResponse<string>("Employee is not active", null, new List<string> { "The employee is either inactive or does not exist." });
        //        }



        //        var salaryOld = await _context.SalaryEmployeeCoos
        //            .Include(x => x.Salary)
        //            .FirstOrDefaultAsync(x => x.EmployeeCooId == employee.Id &&
        //                x.Salary.TransDate.HasValue &&
        //                x.Salary.TransDate.Value.Month == updateSalaryDto.Month &&
        //                x.Salary.TransDate.Value.Year == updateSalaryDto.Year);

        //        decimal basicSalary = employee.Salary ?? 0;

        //        decimal allowedLeave = (decimal)updateSalaryDto.monthlyleave;
        //        decimal daysOffTaken = updateSalaryDto.DaysOff ?? 0;
        //        decimal excessLeave = Math.Max(0, Math.Floor(daysOffTaken - allowedLeave));
        //        decimal daysOffDeduction = (basicSalary / 30) * excessLeave;

        //        decimal transportationAllowance = (employee.Transportation ?? false)
        //            ? updateSalaryDto.Transportioncompensation.HasValue
        //                ? decimal.TryParse(updateSalaryDto.Transportioncompensation.Value.ToString(), out var tempTrans) ? tempTrans : 0
        //                : 0
        //            : 0;

        //        decimal phoneAllowance = (employee.IsMobile ?? false)
        //            ? updateSalaryDto.Mobilecompensation.HasValue
        //                ? decimal.TryParse(updateSalaryDto.Mobilecompensation.Value.ToString(), out var tempPhone) ? tempPhone : 0
        //                : 0
        //            : 0;

        //        decimal laptopAllowance = (employee.Laptop != null && employee.LaptopNavigation?.Name?.ToLower() == "paid laptop")
        //            ? updateSalaryDto.Laptopcompensation.HasValue
        //                ? decimal.TryParse(updateSalaryDto.Laptopcompensation.Value.ToString(), out var tempLaptop) ? tempLaptop : 0
        //                : 0
        //            : 0;
        //        //decimal dsaAllowance = updateSalaryDto.DSA ?? 0;
        //        decimal overtimeAllowance = (decimal)(updateSalaryDto.OverTimeWages ?? 0);
        //        decimal downPaymentAllowance = updateSalaryDto.DownPayment ?? 0;

        //        decimal bonuses = overtimeAllowance + laptopAllowance + transportationAllowance + phoneAllowance;
        //        decimal deductions = daysOffDeduction + downPaymentAllowance;
        //        decimal exchangeRate = (decimal)(updateSalaryDto.Rate );
        //        //decimal? exchangeRate = await _context.UnRates
        //        //    .Where(x => x.MonthNum == DateTime.UtcNow.Month && x.YearNum == DateTime.UtcNow.Year)
        //        //    .Select(x => x.UnRate1)
        //        //    .FirstOrDefaultAsync();

        //        //if (exchangeRate == null || exchangeRate == 0)
        //        //{
        //        //    exchangeRate = await _context.UnRates
        //        //        .Where(x => x.YearNum == DateTime.UtcNow.Year)
        //        //        .OrderByDescending(x => x.MonthNum)
        //        //        .Select(x => x.UnRate1)
        //        //        .FirstOrDefaultAsync();
        //        //}

        //        //if (exchangeRate == null || exchangeRate == 0)
        //        //{
        //        //    return new ApiResponse<string>("Exchange rate not found", null, new List<string> { "Unable to find a valid exchange rate." });
        //        //}

        //        decimal? netSalary = ((basicSalary - daysOffDeduction) + overtimeAllowance) * exchangeRate;
        //        decimal? amount = netSalary + bonuses - downPaymentAllowance;

        //        int totalDaysInMonth = 30;
        //        int daysOn = totalDaysInMonth - (int)daysOffTaken;

        //        if (salaryOld != null && salaryOld.Salary != null && salaryOld.Salary.Ammount.HasValue && salaryOld.Salary.Ammount.Value != 0)
        //        {
        //            salaryOld.Salary.SickLeave = updateSalaryDto.SickLeave;
        //            salaryOld.Salary.DaysOff = updateSalaryDto.DaysOff;
        //            salaryOld.Salary.OverTimeWages = updateSalaryDto.OverTimeWages;
        //            salaryOld.Salary.DownPayment = updateSalaryDto.DownPayment;
        //            salaryOld.Salary.Ammount = (int?)netSalary;
        //            salaryOld.Salary.DaysOn = daysOn;
        //            salaryOld.Salary.TransDate = DateTime.UtcNow;
        //            salaryOld.Salary.Bonuses = (double)bonuses;
        //            salaryOld.Salary.Deductions = (double)deductions;
        //            salaryOld.Salary.UnRate = (int)exchangeRate;
        //            salaryOld.Salary.AnuallLeave = (int?)allowedLeave;
        //        }
        //        else
        //        {
        //            var salaryTran = new SalaryTran
        //            {
        //                SickLeave = updateSalaryDto.SickLeave,
        //                DaysOff = updateSalaryDto.DaysOff,
        //                OverTimeWages = updateSalaryDto.OverTimeWages,
        //                DownPayment = updateSalaryDto.DownPayment,
        //                Ammount = (int?)netSalary,
        //                DaysOn = daysOn,
        //                TransDate = DateTime.UtcNow,
        //                Bonuses = (double?)bonuses,
        //                Deductions = (double?)deductions,
        //                TeamId = employee.TeamId,
        //                ClientId = employee.ClientId,
        //                UnRate = (int?)exchangeRate,
        //                RefNo = employee.EmpId,
        //                SlaryMonth = updateSalaryDto.Month,
        //                SlaryYear = updateSalaryDto.Year,
        //                Laptop = employee.Laptop,
        //                AnuallLeave = (int?)allowedLeave
        //            };

        //            await _context.SalaryTrans.AddAsync(salaryTran);
        //            await _context.SaveChangesAsync();

        //            var salaryEmployeeCoo = new SalaryEmployeeCoo
        //            {
        //                EmployeeCooId = employee.Id,
        //                SalaryId = salaryTran.TransId
        //            };

        //            await _context.SalaryEmployeeCoos.AddAsync(salaryEmployeeCoo);
        //        }

        //        await _context.SaveChangesAsync();

        //        return new ApiResponse<string>($"Salary updated successfully. Net Salary: {netSalary}", null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse<string>("Error occurred while updating the salary", null, new List<string> { ex.Message });
        //    }
        //}


    }
}
