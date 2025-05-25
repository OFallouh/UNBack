using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Net.Mail;
using System.Net;
using UNNew.DTOS.ContractDtos;
using UNNew.DTOS.InsuranceDtos;
using UNNew.Models;
using UNNew.Response;
using System;
using UNNew.Repository.Interfaces;
using System.Linq.Expressions;
using UNNew.Filters;
using System.Reflection;
using System.Text.Json;

namespace UNNew.Repository.Services
{
    public class InsuranceManagmentService : IInsuranceManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public InsuranceManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<ApiResponse<IEnumerable<InsuranceEmployeeDto>>> GetAllInsuranceEmployeeAsync(FilterModel filterModel)
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

                // جلب كل البيانات من قاعدة البيانات أولاً
                var allData = await _context.EmployeeCoos
                    .AsNoTracking()
                    .Include(c => c.Client)
                    .Include(c => c.Team)
                    .Include(c => c.City)
                    .Include(c => c.Coo)
                    .Include(c => c.Emp)
                    .Include(c => c.LaptopNavigation)
                    .Include(c => c.TypeOfContract)
                    .Where(x => x.EndCont >= today && x.Emp.Active)
                    .ToListAsync();

                var query = allData.AsQueryable();
                var Count = allData.Count;

                // تطبيق الفلترة العامة
                if (!string.IsNullOrEmpty(filterModel.GlobalFilter))
                {
                    var globalFilterLower = filterModel.GlobalFilter.ToLower();
                    query = query.Where(emp =>
                        (emp.Client != null && emp.Client.ClientName.ToLower().Contains(globalFilterLower)) ||
                        (emp.Team != null && emp.Team.TeamName.ToLower().Contains(globalFilterLower)) ||
                        (emp.Coo != null && emp.Coo.CooNumber.ToLower().Contains(globalFilterLower)) ||
                        (emp.City != null && emp.City.NameEn.ToLower().Contains(globalFilterLower)) ||
                        (emp.Emp != null && emp.Emp.EmpName.ToLower().Contains(globalFilterLower)) ||
                        (emp.Emp != null && emp.Emp.ArabicName.ToLower().Contains(globalFilterLower)) ||
                        (emp.TypeOfContract != null && emp.TypeOfContract.NmaeEn.ToLower().Contains(globalFilterLower)) ||
                        emp.Tittle.ToLower().Contains(globalFilterLower) ||
                        emp.SuperVisor.ToLower().Contains(globalFilterLower) ||
                        emp.AreaManager.ToLower().Contains(globalFilterLower) ||
                        emp.ProjectName.ToLower().Contains(globalFilterLower) ||
                        (emp.StartLifeDate.HasValue && emp.StartLifeDate.Value.ToString().Contains(globalFilterLower)) ||
                        (emp.EndLifeDate.HasValue && emp.EndLifeDate.Value.ToString().Contains(globalFilterLower)) ||
                        (emp.StartCont.HasValue && emp.StartCont.Value.ToString().Contains(globalFilterLower)) ||
                        (emp.EndCont.HasValue && emp.EndCont.Value.ToString().Contains(globalFilterLower)) ||
                        (emp.SendInsuranceDate.HasValue && emp.SendInsuranceDate.Value.ToString().Contains(globalFilterLower)) ||
                        (emp.InsuranceLife.HasValue && emp.InsuranceLife.Value.ToString().ToLower().Contains(globalFilterLower)) ||
                        (emp.InsuranceMedical.HasValue && emp.InsuranceMedical.Value.ToString().ToLower().Contains(globalFilterLower)) ||
                        GetInsuranceStatus(emp).ToLower().Contains(globalFilterLower)
                    );
                }

                var filteredData = query.ToList();
                var listForOutputOnly = filteredData.Select(emp => new InsuranceEmployeeDto
                {
                    EmpName = emp.Emp?.EmpName ?? "",
                    ArabicName = emp.Emp?.ArabicName ?? "",
                    StartLifeDate = emp.Emp?.StartLifeDate,
                    EndLifeDate = emp.Emp?.EndLifeDate,
                    StartMedicalDate = emp.StartCont,
                    EndMedicalDate = emp.EndCont,
                    InsuranceLife = emp.Emp?.InsuranceLife,
                    InsuranceMedical = emp.Emp?.InsuranceMedical,
                    Stauts = GetInsuranceStatus(emp),
                    InsuranceCardDelivered= emp.Emp.InsuranceCardDelivered,
                    InsuranceCardDeliveredDate= emp.Emp.InsuranceCardDeliveredDate,
                    DaysRemainingLife = emp.EndLifeDate.HasValue ? (emp.EndLifeDate.Value.ToDateTime(new TimeOnly(0, 0)) - today.ToDateTime(new TimeOnly(0, 0))).Days : (int?)null,
                    DaysRemainingMedical = emp.EndCont.HasValue ? (emp.EndCont.Value.ToDateTime(new TimeOnly(0, 0)) - today.ToDateTime(new TimeOnly(0, 0))).Days : (int?)null
                }).ToList();

                // تطبيق الفلاتر الفردية
                if (filterModel?.Filters != null && filterModel.Filters.Count > 0)
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
                        }

                        if (filterValue.TryGetProperty("MatchMode", out JsonElement matchModeElement))
                        {
                            string matchMode = matchModeElement.GetString();
                            JsonElement value = filterValue.GetProperty("Value");
                            PropertyInfo property = typeof(InsuranceEmployeeDto)
                                .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                            if (property != null)
                            {
                                string stringValue = null;
                                bool? boolValue = null;
                                DateOnly? dateOnlyValue = null;
                                int? intValue = null;

                                if (value.ValueKind == JsonValueKind.String)
                                {
                                    stringValue = value.GetString();

                                    // محاولة تحويل القيمة إلى DateOnly
                                    if (DateOnly.TryParse(stringValue, out DateOnly tempDate))
                                    {
                                        dateOnlyValue = tempDate;
                                    }
                                }
                                else if (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
                                {
                                    boolValue = value.GetBoolean();
                                }
                                else if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var tempInt))
                                {
                                    intValue = tempInt;
                                }

                                string filterValueLower = stringValue?.ToLower();

                                listForOutputOnly = listForOutputOnly.Where(dto =>
                                {
                                    var propertyValue = property.GetValue(dto);
                                    if (propertyValue == null) return false;

                                    var propString = propertyValue.ToString()?.ToLower();
                                    var propBool = propertyValue as bool?;
                                    var propInt = propertyValue as int?;
                                    var propDate = propertyValue as DateOnly?;

                                    switch (matchMode.ToLower())
                                    {
                                        case "equals":
                                            if (stringValue != null)
                                                return propString == filterValueLower;
                                            if (boolValue.HasValue)
                                                return propBool == boolValue.Value;
                                            if (dateOnlyValue.HasValue)
                                                return propDate == dateOnlyValue.Value;
                                            if (intValue.HasValue)
                                                return propInt == intValue.Value;
                                            return false;

                                        case "notequals":
                                            if (stringValue != null)
                                                return propString != filterValueLower;
                                            if (boolValue.HasValue)
                                                return propBool != boolValue.Value;
                                            if (dateOnlyValue.HasValue)
                                                return propDate != dateOnlyValue.Value;
                                            if (intValue.HasValue)
                                                return propInt != intValue.Value;
                                            return false;

                                        case "startswith":
                                            return stringValue != null && propString != null && propString.StartsWith(filterValueLower);

                                        case "endswith":
                                            return stringValue != null && propString != null && propString.EndsWith(filterValueLower);

                                        case "contains":
                                            return stringValue != null && propString != null && propString.Contains(filterValueLower);

                                        case "notcontains":
                                            return stringValue != null && propString != null && !propString.Contains(filterValueLower);

                                        case "lt":
                                            if (intValue.HasValue)
                                                return propInt < intValue.Value;
                                            if (dateOnlyValue.HasValue)
                                                return propDate < dateOnlyValue.Value;
                                            return false;

                                        case "lte":
                                            if (intValue.HasValue)
                                                return propInt <= intValue.Value;
                                            if (dateOnlyValue.HasValue)
                                                return propDate <= dateOnlyValue.Value;
                                            return false;

                                        case "gt":
                                            if (intValue.HasValue)
                                                return propInt > intValue.Value;
                                            if (dateOnlyValue.HasValue)
                                                return propDate > dateOnlyValue.Value;
                                            return false;

                                        case "gte":
                                            if (intValue.HasValue)
                                                return propInt >= intValue.Value;
                                            if (dateOnlyValue.HasValue)
                                                return propDate >= dateOnlyValue.Value;
                                            return false;

                                        case "dateis":
                                            return dateOnlyValue.HasValue && propDate == dateOnlyValue.Value;

                                        case "dateisnot":
                                            return dateOnlyValue.HasValue && propDate != dateOnlyValue.Value;

                                        case "datebefore":
                                            return dateOnlyValue.HasValue && propDate < dateOnlyValue.Value;

                                        case "dateafter":
                                            return dateOnlyValue.HasValue && propDate > dateOnlyValue.Value;

                                        default:
                                            return true;
                                    }
                                }).ToList();
                            }

                        }
                    }
                }

                int finalCount = listForOutputOnly.Count();

                // تطبيق الترتيب
                if (filterModel?.MultiSortMeta?.Count > 0)
                {
                    IOrderedEnumerable<InsuranceEmployeeDto> orderedData = null;
                    foreach (var sortMeta in filterModel.MultiSortMeta)
                    {
                        var property = typeof(InsuranceEmployeeDto).GetProperty(sortMeta.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (property != null)
                        {
                            if (orderedData == null)
                            {
                                orderedData = sortMeta.Order == 1
                                    ? listForOutputOnly.OrderBy(x => property.GetValue(x))
                                    : listForOutputOnly.OrderByDescending(x => property.GetValue(x));
                            }
                            else
                            {
                                orderedData = sortMeta.Order == 1
                                    ? orderedData.ThenBy(x => property.GetValue(x))
                                    : orderedData.ThenByDescending(x => property.GetValue(x));
                            }
                        }
                    }
                    if (orderedData != null)
                        listForOutputOnly = orderedData.ToList();
                }

                // تطبيق التقسيم الصفحي
                int skipCount = filterModel?.First ?? 0;
                int takeCount = filterModel?.Rows ?? int.MaxValue;
                var pagedData = listForOutputOnly.Skip(skipCount).Take(takeCount).ToList();

                return new ApiResponse<IEnumerable<InsuranceEmployeeDto>>("InsuranceEmployees fetched successfully", pagedData, null, finalCount);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<InsuranceEmployeeDto>>("Error occurred while fetching InsuranceEmployees", null, new List<string> { ex.Message });
            }
        }

        private string GetInsuranceStatus(EmployeeCoo emp)
        {
            if (emp.InsuranceLife.HasValue && emp.InsuranceMedical.HasValue)
            {
                return emp.InsuranceLife.Value && emp.InsuranceMedical.Value ? "Insured with both Life and Medical"
                       : emp.InsuranceLife.Value ? "Insured with Life only"
                       : emp.InsuranceMedical.Value ? "Insured with Medical only"
                       : "Not Insured";
            }
            else
            {
                return emp.SendInsuranceDate.HasValue && emp.Emp?.EndLifeDate == null && emp.Emp?.StartLifeDate == null
                       ? "Awaiting Insurance Company"
                       : "Not Insured";
            }
        }
        //public async Task<ApiResponse<IEnumerable<InsuranceEmployeeDto>>> GetAllInsuranceEmployeeAsync(FilterModel filterModel)
        //{
        //    try
        //    {
        //        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        //        // جلب كل البيانات من قاعدة البيانات أولاً
        //        var allData = await _context.EmployeeCoos
        //            .AsNoTracking()
        //            .Include(c => c.Client)
        //            .Include(c => c.Team)
        //            .Include(c => c.City)
        //            .Include(c => c.Coo)
        //            .Include(c => c.Emp)
        //            .Include(c => c.LaptopNavigation)
        //            .Include(c => c.TypeOfContract)
        //            .Where(x => x.EndCont >= today && x.Emp.Active)
        //            .ToListAsync();

        //        var query = allData.AsQueryable();
        //        var Count = allData.Count;

        //        // تطبيق الفلترة العامة
        //        if (!string.IsNullOrEmpty(filterModel.GlobalFilter))
        //        {
        //            query = query.Where(emp =>
        //                (emp.Client != null && emp.Client.ClientName.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase)) ||
        //                (emp.Team != null && emp.Team.TeamName.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase)) ||
        //                (emp.Coo != null && emp.Coo.CooNumber.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase)) ||
        //                (emp.City != null && emp.City.NameEn.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase)) ||
        //                (emp.Emp != null && emp.Emp.EmpName.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase)) ||
        //                (emp.Emp != null && emp.Emp.ArabicName.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase)) ||
        //                (emp.TypeOfContract != null && emp.TypeOfContract.NmaeEn.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase)) ||
        //                emp.Tittle.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase) ||
        //                emp.SuperVisor.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase) ||
        //                emp.AreaManager.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase) ||
        //                emp.ProjectName.Contains(filterModel.GlobalFilter, StringComparison.OrdinalIgnoreCase));
        //        }

        //        // تطبيق الفلاتر الفردية
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
        //                        case "id":
        //                            query = ApplyNumericFilterInMemory(query, c => c.Id, filterCriteria);
        //                            break;
        //                        case "empname":
        //                            query = ApplyStringFilterInMemory(query, c => c.Emp?.EmpName ?? "", filterCriteria);
        //                            break;
        //                        case "arabicname":
        //                            query = ApplyStringFilterInMemory(query, c => c.Emp?.ArabicName ?? "", filterCriteria);
        //                            break;
        //                        case "insurancelife":
        //                            query = ApplyBooleanFilterInMemory(query, c => c.InsuranceLife, filterCriteria);
        //                            break;
        //                        case "startlifedate":
        //                            query = ApplyDateFilterInMemory(query, c => c.StartLifeDate, filterCriteria);
        //                            break;
        //                        case "endlifedate":
        //                            query = ApplyDateFilterInMemory(query, c => c.EndLifeDate, filterCriteria);
        //                            break;
        //                        case "insurancemedical":
        //                            query = ApplyBooleanFilterInMemory(query, c => c.InsuranceMedical, filterCriteria);
        //                            break;
        //                        case "startmedicaldate":
        //                            query = ApplyDateFilterInMemory(query, c => c.StartCont, filterCriteria);
        //                            break;
        //                        case "endmedicaldate":
        //                            query = ApplyDateFilterInMemory(query, c => c.EndCont, filterCriteria);
        //                            break;
        //                        case "sendinsurancedate":
        //                            query = ApplyDateFilterInMemory(query, c => c.SendInsuranceDate, filterCriteria);
        //                            break;
        //                        case "status":
        //                            query = ApplyInsuranceStatusFilterInMemory(query, filterCriteria);
        //                            break;
        //                    }
        //                }
        //            }
        //        }

        //        // تطبيق الترتيب
        //        var sortedData = query.AsEnumerable();
        //        if (filterModel.MultiSortMeta != null && filterModel.MultiSortMeta.Any())
        //        {
        //            IOrderedEnumerable<EmployeeCoo> orderedData = null;
        //            foreach (var sortMeta in filterModel.MultiSortMeta)
        //            {
        //                var propertyName = sortMeta.Field;
        //                var propertyInfo = typeof(EmployeeCoo).GetProperty(propertyName,
        //                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        //                if (propertyInfo != null)
        //                {
        //                    if (orderedData == null)
        //                    {
        //                        orderedData = sortMeta.Order == 1 ?
        //                            sortedData.OrderBy(x => propertyInfo.GetValue(x, null)) :
        //                            sortedData.OrderByDescending(x => propertyInfo.GetValue(x, null));
        //                    }
        //                    else
        //                    {
        //                        orderedData = sortMeta.Order == 1 ?
        //                            orderedData.ThenBy(x => propertyInfo.GetValue(x, null)) :
        //                            orderedData.ThenByDescending(x => propertyInfo.GetValue(x, null));
        //                    }
        //                }
        //            }

        //            if (orderedData != null)
        //            {
        //                sortedData = orderedData;
        //            }
        //        }

        //        // تطبيق التقسيم الصفحي
        //        var rowsToDisplay = filterModel.Rows > 0 ? filterModel.Rows : 5;
        //        var pagedData = sortedData
        //            .Skip(filterModel.First)
        //            .Take(rowsToDisplay)
        //            .ToList();

        //        if (!pagedData.Any())
        //        {
        //            return new ApiResponse<IEnumerable<InsuranceEmployeeDto>>("No InsuranceEmployees found", null, new List<string> { "No data available" });
        //        }

        //        var InsuranceEmployeeDtos = pagedData.Select(emp => new InsuranceEmployeeDto
        //        {
        //            EmpName = emp.Emp?.EmpName ?? "",
        //            ArabicName = emp.Emp?.ArabicName ?? "",
        //            StartLifeDate = emp.Emp.StartLifeDate,
        //            EndLifeDate = emp.Emp.EndLifeDate,
        //            StartMedicalDate = emp.StartCont,
        //            EndMedicalDate = emp.EndCont,
        //            InsuranceLife = emp.InsuranceLife,
        //            InsuranceMedical = emp.InsuranceMedical,
        //            SendInsuranceDate = emp.SendInsuranceDate,
        //            Stauts = emp.InsuranceLife.HasValue && emp.InsuranceMedical.HasValue
        //                ? (emp.InsuranceLife.Value && emp.InsuranceMedical.Value ? "Insured with both Life and Medical"
        //                   : emp.InsuranceLife.Value ? "Insured with Life only"
        //                   : emp.InsuranceMedical.Value ? "Insured with Medical only"
        //                   : "Not Insured")
        //                : (emp.SendInsuranceDate.HasValue && emp.Emp.EndLifeDate == null && emp.Emp.StartLifeDate == null ? "Awaiting Insurance Company"
        //                   : "Not Insured"),
        //            DaysRemainingLife = emp.EndLifeDate.HasValue ? (emp.EndLifeDate.Value.ToDateTime(new TimeOnly(0, 0)) - today.ToDateTime(new TimeOnly(0, 0))).Days : (int?)null,
        //            DaysRemainingMedical = emp.EndCont.HasValue ? (emp.EndCont.Value.ToDateTime(new TimeOnly(0, 0)) - today.ToDateTime(new TimeOnly(0, 0))).Days : (int?)null
        //        }).ToList();

        //        return new ApiResponse<IEnumerable<InsuranceEmployeeDto>>("InsuranceEmployees fetched successfully", InsuranceEmployeeDtos, null, Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse<IEnumerable<InsuranceEmployeeDto>>("Error occurred while fetching InsuranceEmployees", null, new List<string> { ex.Message });
        //    }
        //}

        //// دوال الفلترة المعدلة للعمل في الذاكرة
        //private IQueryable<EmployeeCoo> ApplyStringFilterInMemory(IQueryable<EmployeeCoo> query, Func<EmployeeCoo, string> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    var value = filterCriteria.Value?.ToLower();

        //    switch (matchMode)
        //    {
        //        case "startswith":
        //            return query.Where(x => propertySelector(x)?.ToLower().StartsWith(value) ?? false).AsQueryable();
        //        case "contains":
        //            return query.Where(x => propertySelector(x)?.ToLower().Contains(value) ?? false).AsQueryable();
        //        case "endswith":
        //            return query.Where(x => propertySelector(x)?.ToLower().EndsWith(value) ?? false).AsQueryable();
        //        case "equals":
        //            return query.Where(x => propertySelector(x)?.ToLower() == value).AsQueryable();
        //        case "notequals":
        //            return query.Where(x => propertySelector(x)?.ToLower() != value).AsQueryable();
        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<EmployeeCoo> ApplyNumericFilterInMemory(IQueryable<EmployeeCoo> query, Func<EmployeeCoo, int?> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (!int.TryParse(filterCriteria.Value, out var numericValue))
        //        return query;

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(x => propertySelector(x) == numericValue).AsQueryable();
        //        case "notequals":
        //            return query.Where(x => propertySelector(x) != numericValue).AsQueryable();
        //        case "lt":
        //            return query.Where(x => propertySelector(x) < numericValue).AsQueryable();
        //        case "lte":
        //            return query.Where(x => propertySelector(x) <= numericValue).AsQueryable();
        //        case "gt":
        //            return query.Where(x => propertySelector(x) > numericValue).AsQueryable();
        //        case "gte":
        //            return query.Where(x => propertySelector(x) >= numericValue).AsQueryable();
        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<EmployeeCoo> ApplyDateFilterInMemory(IQueryable<EmployeeCoo> query, Func<EmployeeCoo, DateOnly?> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (!DateOnly.TryParse(filterCriteria.Value, out var dateValue))
        //        return query;

        //    switch (matchMode)
        //    {
        //        case "equals":
        //        case "dateis":
        //            return query.Where(x => propertySelector(x) == dateValue).AsQueryable();
        //        case "notequals":
        //        case "dateisnot":
        //            return query.Where(x => propertySelector(x) != dateValue).AsQueryable();
        //        case "before":
        //        case "datebefore":
        //            return query.Where(x => propertySelector(x) < dateValue).AsQueryable();
        //        case "after":
        //        case "dateafter":
        //            return query.Where(x => propertySelector(x) > dateValue).AsQueryable();
        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<EmployeeCoo> ApplyBooleanFilterInMemory(IQueryable<EmployeeCoo> query, Func<EmployeeCoo, bool?> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (!bool.TryParse(filterCriteria.Value, out var boolValue))
        //        return query;

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(x => propertySelector(x) == boolValue).AsQueryable();
        //        case "notequals":
        //            return query.Where(x => propertySelector(x) != boolValue).AsQueryable();
        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<EmployeeCoo> ApplyInsuranceStatusFilterInMemory(IQueryable<EmployeeCoo> query, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    var value = filterCriteria.Value?.ToLower();

        //    return query.Where(emp =>
        //    {
        //        var status = emp.InsuranceLife.HasValue && emp.InsuranceMedical.HasValue
        //            ? (emp.InsuranceLife.Value && emp.InsuranceMedical.Value ? "insured with both life and medical"
        //               : emp.InsuranceLife.Value ? "insured with life only"
        //               : emp.InsuranceMedical.Value ? "insured with medical only"
        //               : "not insured")
        //            : (emp.SendInsuranceDate.HasValue && emp.Emp?.EndLifeDate == null && emp.Emp?.StartLifeDate == null
        //               ? "awaiting insurance company"
        //               : "not insured");

        //        status = status.ToLower();

        //        return matchMode switch
        //        {
        //            "equals" => status == value,
        //            "notequals" => status != value,
        //            "contains" => status.Contains(value),
        //            "startswith" => status.StartsWith(value),
        //            "endswith" => status.EndsWith(value),
        //            _ => true
        //        };
        //    }).AsQueryable();
        //}
        //public async Task<ApiResponse<IEnumerable<InsuranceEmployeeDto>>> GetAllInsuranceEmployeeAsync(FilterModel filterModel)
        //{
        //    try
        //    {
        //        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        //        var query = _context.EmployeeCoos
        //            .AsNoTracking()
        //            .Include(c => c.Client)
        //            .Include(c => c.Team)
        //            .Include(c => c.City)
        //            .Include(c => c.Coo)
        //            .Include(c => c.Emp)
        //            .Include(c => c.LaptopNavigation)
        //            .Include(c => c.TypeOfContract)
        //            .Where(x => x.EndCont >= today && x.Emp.Active)
        //            .AsQueryable();

        //        var Count = query.Count();

        //        // Apply global filter
        //        if (!string.IsNullOrEmpty(filterModel.GlobalFilter))
        //        {
        //            query = query.Where(emp =>
        //                (emp.Client != null && emp.Client.ClientName.Contains(filterModel.GlobalFilter)) ||
        //                (emp.Team != null && emp.Team.TeamName.Contains(filterModel.GlobalFilter)) ||
        //                (emp.Coo != null && emp.Coo.CooNumber.Contains(filterModel.GlobalFilter)) ||
        //                (emp.City != null && emp.City.NameEn.Contains(filterModel.GlobalFilter)) ||
        //                (emp.Emp != null && emp.Emp.EmpName.Contains(filterModel.GlobalFilter)) ||
        //                (emp.Emp != null && emp.Emp.ArabicName.Contains(filterModel.GlobalFilter)) ||
        //                (emp.TypeOfContract != null && emp.TypeOfContract.NmaeEn.Contains(filterModel.GlobalFilter)) ||
        //                emp.Tittle.Contains(filterModel.GlobalFilter) ||
        //                emp.SuperVisor.Contains(filterModel.GlobalFilter) ||
        //                emp.AreaManager.Contains(filterModel.GlobalFilter) ||
        //                emp.ProjectName.Contains(filterModel.GlobalFilter));
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
        //                        case "id":
        //                            query = ApplyNumericFilter(query, c => c.Id, filterCriteria);
        //                            break;
        //                        case "empname":
        //                            query = ApplyStringFilter(query, c => c.Emp != null ? c.Emp.EmpName : "", filterCriteria);
        //                            break;
        //                        case "arabicname":
        //                            query = ApplyStringFilter(query, c => c.Emp != null ? c.Emp.ArabicName : "", filterCriteria);
        //                            break;
        //                        case "insurancelife":
        //                            query = ApplyBooleanFilter(query, c => c.InsuranceLife, filterCriteria);
        //                            break;
        //                        case "startlifedate":
        //                            query = ApplyDateFilter(query, c => c.StartLifeDate, filterCriteria);
        //                            break;
        //                        case "endlifedate":
        //                            query = ApplyDateFilter(query, c => c.EndLifeDate, filterCriteria);
        //                            break;
        //                        case "insurancemedical":
        //                            query = ApplyBooleanFilter(query, c => c.InsuranceMedical, filterCriteria);
        //                            break;
        //                        case "startmedicaldate":
        //                            query = ApplyDateFilter(query, c => c.StartCont, filterCriteria);
        //                            break;
        //                        case "endmedicaldate":
        //                            query = ApplyDateFilter(query, c => c.EndCont, filterCriteria);
        //                            break;
        //                        case "sendinsurancedate":
        //                            query = ApplyDateFilter(query, c => c.SendInsuranceDate, filterCriteria);
        //                            break;
        //                        case "status":
        //                            query = ApplyInsuranceStatusFilter(query, filterCriteria);
        //                            break;
        //                    }
        //                }
        //            }
        //        }

        //        // Apply sorting
        //        if (filterModel.MultiSortMeta != null && filterModel.MultiSortMeta.Any())
        //        {
        //            foreach (var sortMeta in filterModel.MultiSortMeta)
        //            {
        //                var propertyName = sortMeta.Field.ToLower();
        //                var propertyInfo = typeof(EmployeeCoo).GetProperties()
        //                    .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

        //                if (propertyInfo != null)
        //                {
        //                    var parameter = Expression.Parameter(typeof(EmployeeCoo), "emp");
        //                    var propertyExpression = Expression.Property(parameter, propertyInfo);
        //                    var lambda = Expression.Lambda(propertyExpression, parameter);

        //                    var method = sortMeta.Order == -1 ? "OrderByDescending" : "OrderBy";
        //                    var resultExpression = Expression.Call(
        //                        typeof(Queryable),
        //                        method,
        //                        new Type[] { typeof(EmployeeCoo), propertyInfo.PropertyType },
        //                        query.Expression,
        //                        lambda
        //                    );

        //                    query = query.Provider.CreateQuery<EmployeeCoo>(resultExpression);
        //                }
        //            }
        //        }

        //        // Pagination
        //        var rowsToDisplay = filterModel.Rows > 0 ? filterModel.Rows : 5;

        //        var InsuranceEmployee = await query
        //            .Skip(filterModel.First)
        //            .Take(rowsToDisplay)
        //            .ToListAsync();

        //        if (InsuranceEmployee == null || !InsuranceEmployee.Any())
        //        {
        //            return new ApiResponse<IEnumerable<InsuranceEmployeeDto>>("No InsuranceEmployees found", null, new List<string> { "No data available" });
        //        }

        //        var InsuranceEmployeeDtos = InsuranceEmployee.Select(emp => new InsuranceEmployeeDto
        //        {
        //            EmpName = emp.Emp?.EmpName ?? "",
        //            ArabicName = emp.Emp?.ArabicName ?? "",
        //            StartLifeDate = emp.Emp.StartLifeDate,
        //            EndLifeDate = emp.Emp.EndLifeDate,
        //            StartMedicalDate = emp.StartCont,
        //            EndMedicalDate = emp.EndCont,
        //            InsuranceLife = emp.InsuranceLife,
        //            InsuranceMedical = emp.InsuranceMedical,
        //            SendInsuranceDate = emp.SendInsuranceDate,
        //            Stauts = emp.InsuranceLife.HasValue && emp.InsuranceMedical.HasValue
        //                ? (emp.InsuranceLife.Value && emp.InsuranceMedical.Value ? "Insured with both Life and Medical" // تأمين شامل
        //                   : emp.InsuranceLife.Value ? "Insured with Life only" // تأمين حياة فقط
        //                   : emp.InsuranceMedical.Value ? "Insured with Medical only" // تأمين صحي فقط
        //                   : "Not Insured") // لا يوجد تأمين
        //                : (emp.SendInsuranceDate.HasValue && emp.Emp.EndLifeDate == null && emp.Emp.StartLifeDate == null ? "Awaiting Insurance Company" // انتظار شركة التأمين
        //                   : "Not Insured"), // لا يوجد تأمين

        //            DaysRemainingLife = emp.EndLifeDate.HasValue ? (emp.EndLifeDate.Value.ToDateTime(new TimeOnly(0, 0)) - today.ToDateTime(new TimeOnly(0, 0))).Days : (int?)null,
        //            DaysRemainingMedical = emp.EndCont.HasValue ? (emp.EndCont.Value.ToDateTime(new TimeOnly(0, 0)) - today.ToDateTime(new TimeOnly(0, 0))).Days : (int?)null
        //        }).ToList();

        //        return new ApiResponse<IEnumerable<InsuranceEmployeeDto>>("InsuranceEmployees fetched successfully", InsuranceEmployeeDtos, null, Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse<IEnumerable<InsuranceEmployeeDto>>("Error occurred while fetching InsuranceEmployees", null, new List<string> { ex.Message });
        //    }
        //}
        //private IQueryable<EmployeeCoo> ApplyStringFilter(IQueryable<EmployeeCoo> query, Expression<Func<EmployeeCoo, string>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    var value = filterCriteria.Value;

        //    switch (matchMode)
        //    {
        //        case "startswith":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.Call(
        //                    propertySelector.Body,
        //                    typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
        //                Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "contains":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.Call(
        //                    propertySelector.Body,
        //                    typeof(string).GetMethod("Contains", new[] { typeof(string) }),
        //                Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "endswith":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.Call(
        //                    propertySelector.Body,
        //                    typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
        //                Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "notequals": // تمت إضافة هذه الحالة
        //        case "notequal": // يمكن دعم الاسمين
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<EmployeeCoo> ApplyNumericFilter(IQueryable<EmployeeCoo> query, Expression<Func<EmployeeCoo, int?>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (!int.TryParse(filterCriteria.Value, out var numericValue))
        //        return query;

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "notequals":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "lt":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.LessThan(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "lte":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.LessThanOrEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "gt":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.GreaterThan(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "gte":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.GreaterThanOrEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<EmployeeCoo> ApplyDateFilter(IQueryable<EmployeeCoo> query, Expression<Func<EmployeeCoo, DateOnly?>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (!DateOnly.TryParse(filterCriteria.Value, out var dateValue))
        //        return query;

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        case "notequals":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        case "before":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.LessThan(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        case "after":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.GreaterThan(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<EmployeeCoo> ApplyBooleanFilter(IQueryable<EmployeeCoo> query, Expression<Func<EmployeeCoo, bool?>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (!bool.TryParse(filterCriteria.Value, out var boolValue))
        //        return query;

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(boolValue)),
        //                propertySelector.Parameters));

        //        case "notequals":
        //            return query.Where(Expression.Lambda<Func<EmployeeCoo, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(boolValue)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<EmployeeCoo> ApplyInsuranceStatusFilter(IQueryable<EmployeeCoo> query, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    var value = filterCriteria.Value?.ToLower();
        //    var today = DateOnly.FromDateTime(DateTime.UtcNow);

        //    return query.Where(emp =>
        //        (matchMode == "equals" &&
        //         (emp.InsuranceLife.HasValue && emp.InsuranceMedical.HasValue
        //            ? (emp.InsuranceLife.Value && emp.InsuranceMedical.Value ? "insured with both life and medical"
        //               : emp.InsuranceLife.Value ? "insured with life only"
        //               : emp.InsuranceMedical.Value ? "insured with medical only"
        //               : "not insured")
        //            : (emp.SendInsuranceDate.HasValue && emp.Emp.EndLifeDate == null && emp.Emp.StartLifeDate == null ? "awaiting insurance company"
        //               : "not insured")).ToLower() == value) ||
        //        (matchMode == "notequals" &&
        //         (emp.InsuranceLife.HasValue && emp.InsuranceMedical.HasValue
        //            ? (emp.InsuranceLife.Value && emp.InsuranceMedical.Value ? "insured with both life and medical"
        //               : emp.InsuranceLife.Value ? "insured with life only"
        //               : emp.InsuranceMedical.Value ? "insured with medical only"
        //               : "not insured")
        //            : (emp.SendInsuranceDate.HasValue && emp.Emp.EndLifeDate == null && emp.Emp.StartLifeDate == null ? "awaiting insurance company"
        //               : "not insured")).ToLower() != value) ||
        //        (matchMode == "contains" &&
        //         (emp.InsuranceLife.HasValue && emp.InsuranceMedical.HasValue
        //            ? (emp.InsuranceLife.Value && emp.InsuranceMedical.Value ? "insured with both life and medical"
        //               : emp.InsuranceLife.Value ? "insured with life only"
        //               : emp.InsuranceMedical.Value ? "insured with medical only"
        //               : "not insured")
        //            : (emp.SendInsuranceDate.HasValue && emp.Emp.EndLifeDate == null && emp.Emp.StartLifeDate == null ? "awaiting insurance company"
        //               : "not insured")).ToLower().Contains(value)) ||
        //        (matchMode == "startswith" &&
        //         (emp.InsuranceLife.HasValue && emp.InsuranceMedical.HasValue
        //            ? (emp.InsuranceLife.Value && emp.InsuranceMedical.Value ? "insured with both life and medical"
        //               : emp.InsuranceLife.Value ? "insured with life only"
        //               : emp.InsuranceMedical.Value ? "insured with medical only"
        //               : "not insured")
        //            : (emp.SendInsuranceDate.HasValue && emp.Emp.EndLifeDate == null && emp.Emp.StartLifeDate == null ? "awaiting insurance company"
        //               : "not insured")).ToLower().StartsWith(value)) ||
        //        (matchMode == "endswith" &&
        //         (emp.InsuranceLife.HasValue && emp.InsuranceMedical.HasValue
        //            ? (emp.InsuranceLife.Value && emp.InsuranceMedical.Value ? "insured with both life and medical"
        //               : emp.InsuranceLife.Value ? "insured with life only"
        //               : emp.InsuranceMedical.Value ? "insured with medical only"
        //               : "not insured")
        //            : (emp.SendInsuranceDate.HasValue && emp.Emp.EndLifeDate == null && emp.Emp.StartLifeDate == null ? "awaiting insurance company"
        //               : "not insured")).ToLower().EndsWith(value)) ||
        //        string.IsNullOrEmpty(matchMode)
        //    );
        //}
        public async Task<ApiResponse<InsuranceEmployeeDto>> GetInsuranceEmployeeByIdAsync(int EmployeeId)
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
                var InsuranceEmployee = await _context.EmployeeCoos
                    .AsNoTracking()
                    .Include(x => x.Emp)
                    .FirstOrDefaultAsync(x => x.EmpId == EmployeeId && x.EndCont >= today && x.Emp.Active);

                if (InsuranceEmployee == null)
                {
                    return new ApiResponse<InsuranceEmployeeDto>("Employee is not Active", null, new List<string> { "Employee is not Active" });
                }

                var InsuranceEmployeeDto = new InsuranceEmployeeDto
                {
                    EmployeeId = InsuranceEmployee.Emp.RefNo,
                    StartLifeDate = InsuranceEmployee.Emp.StartLifeDate,
                    EndLifeDate = InsuranceEmployee.Emp.EndLifeDate,
                    StartMedicalDate = InsuranceEmployee.StartCont,
                    EndMedicalDate = InsuranceEmployee.EndCont,
                    InsuranceLife = InsuranceEmployee.Emp.InsuranceLife,
                    InsuranceMedical = InsuranceEmployee.Emp.InsuranceMedical,
                    EmpName = InsuranceEmployee.Emp?.EmpName ?? " ",
                    ArabicName = InsuranceEmployee.Emp?.ArabicName ?? "",
                    InsuranceCardDelivered = InsuranceEmployee?.Emp?.InsuranceCardDelivered,
                    InsuranceCardDeliveredDate = InsuranceEmployee?.Emp?.InsuranceCardDeliveredDate,
                    Stauts = InsuranceEmployee.InsuranceLife.HasValue && InsuranceEmployee.InsuranceMedical.HasValue
                        ? (InsuranceEmployee.InsuranceLife.Value && InsuranceEmployee.InsuranceMedical.Value ? "Insured with both Life and Medical" // تأمين شامل
                           : InsuranceEmployee.InsuranceLife.Value ? "Insured with Life only" // تأمين حياة فقط
                           : InsuranceEmployee.InsuranceMedical.Value ? "Insured with Medical only" // تأمين صحي فقط
                           : "Not Insured") // لا يوجد تأمين
                        : (InsuranceEmployee.SendInsuranceDate.HasValue && InsuranceEmployee.Emp.EndLifeDate == null && InsuranceEmployee.Emp.StartLifeDate == null ? "Awaiting Insurance Company" // انتظار شركة التأمين
                           : "Not Insured"), // لا يوجد تأمين

                    // Calculate days remaining for InsuranceLife
                    DaysRemainingLife = InsuranceEmployee.EndLifeDate.HasValue ? (InsuranceEmployee.EndLifeDate.Value.ToDateTime(new TimeOnly(0, 0)) - today.ToDateTime(new TimeOnly(0, 0))).Days : (int?)null,

                    // Calculate days remaining for InsuranceMedical
                    DaysRemainingMedical = InsuranceEmployee.EndCont.HasValue ? (InsuranceEmployee.EndCont.Value.ToDateTime(new TimeOnly(0, 0)) - today.ToDateTime(new TimeOnly(0, 0))).Days : (int?)null
                };

                return new ApiResponse<InsuranceEmployeeDto>("InsuranceEmployeeDto fetched successfully", InsuranceEmployeeDto, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<InsuranceEmployeeDto>("Error occurred while fetching InsuranceEmployeeDto", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> UpdateInsuranceEmployeeAsync(UpdateInsuranceDto updateInsuranceDto)
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

                // البحث عن الموظف
                var InsuranceEmployeeDto = await _context.EmployeeCoos
                    .Include(x => x.Emp)
                    .FirstOrDefaultAsync(x => x.EmpId == updateInsuranceDto.EmployeeId && x.EndCont >= today && x.Emp.Active);

                if (InsuranceEmployeeDto == null)
                {
                    return new ApiResponse<string>("Employee is not Active", null, new List<string> { "Employee is not Active" });
                }

                // ✅ تحقق من تواريخ التأمين على الحياة
                if (updateInsuranceDto.StartLifeDate.HasValue && updateInsuranceDto.EndLifeDate.HasValue &&
                    updateInsuranceDto.StartLifeDate > updateInsuranceDto.EndLifeDate)
                {
                    return new ApiResponse<string>(
                        "StartLifeDate cannot be after EndLifeDate.",
                        null,
                        new List<string> { "StartLifeDate must be before or equal to EndLifeDate." });
                }

                // تحديث بيانات الـ Emp المرتبطة بالموظف
                if (InsuranceEmployeeDto.Emp != null)
                {
                    InsuranceEmployeeDto.Emp.InsuranceMedical = updateInsuranceDto.InsuranceMedical;
                    InsuranceEmployeeDto.Emp.InsuranceLife = updateInsuranceDto.InsuranceLife;
                    InsuranceEmployeeDto.Emp.StartLifeDate = updateInsuranceDto.StartLifeDate;
                    InsuranceEmployeeDto.Emp.EndLifeDate = updateInsuranceDto.EndLifeDate;
                    InsuranceEmployeeDto.Emp.InsuranceCardDelivered = updateInsuranceDto.InsuranceCardDelivered;
                    InsuranceEmployeeDto.Emp.InsuranceCardDeliveredDate = updateInsuranceDto.InsuranceCardDeliveredDate;
                }

                // حفظ التغييرات في قاعدة البيانات
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("InsuranceEmployeeDto updated successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while updating the InsuranceEmployeeDto", null, new List<string> { ex.Message });
            }
        }


        //public async Task<ApiResponse<string>> SendLifeInsuranceNotificationsAsync()
        //{
        //    try
        //    {
        //        DateOnly targetDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));

        //        // Retrieve employees whose life insurance will expire in 10 days
        //        var employees = await _context.EmployeeCoos
        //            .AsNoTracking()
        //            .Include(e => e.Emp)
        //            .Where(e => e.EndLifeDate == targetDate &&
        //                        e.Emp.Active == true &&  // Employee is active
        //                        (e.EndCont > DateOnly.FromDateTime(DateTime.UtcNow))) // Contract is still valid
        //            .ToListAsync();

        //        if (!employees.Any())
        //        {
        //            return new ApiResponse<string>("No employees found with life insurance expiring in 10 days.", null, null);
        //        }

        //        // Retrieve users with RoleId = 1 (Admin)
        //        var adminEmails = await _context.UserRoles
        //            .Where(ur => ur.RoleId == 1)

        //            .ToListAsync();

        //        if (!adminEmails.Any())
        //        {
        //            return new ApiResponse<string>("No admins available to receive notifications.", null, null);
        //        }

        //        //// Send notification to each admin
        //        //foreach (var email in adminEmails)
        //        //{
        //        //    SendEmailNotification(email, employees);
        //        //}

        //        return new ApiResponse<string>("Notifications sent successfully.", null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse<string>("An error occurred while sending notifications.", null, new List<string> { ex.Message });
        //    }
        //}

        //private void SendEmailNotification(string email, List<EmployeeCoo> employees)
        //{
        //    try
        //    {
        //        using (var client = new SmtpClient(_configuration["SMTP:Host"], int.Parse(_configuration["SMTP:Port"])))
        //        {
        //            client.Credentials = new NetworkCredential(_configuration["SMTP:Username"], _configuration["SMTP:Password"]);
        //            client.EnableSsl = bool.Parse(_configuration["SMTP:EnableSSL"]);

        //            var body = "<h3>Employees with expiring life insurance in 10 days:</h3><ul>";
        //            foreach (var employee in employees)
        //            {
        //                body += $"<li>{employee.Emp.EmpName} - Expiry Date: {employee.EndLifeDate}</li>";
        //            }
        //            body += "</ul>";

        //            var mailMessage = new MailMessage
        //            {
        //                From = new MailAddress(_configuration["SMTP:FromEmail"]),
        //                Subject = "Life Insurance Expiry Notification",
        //                Body = body,
        //                IsBodyHtml = true,
        //            };
        //            mailMessage.To.Add(email);

        //            client.Send(mailMessage);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
        //    }
        //}


    }
}
