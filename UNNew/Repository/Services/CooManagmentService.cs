using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using UNNew.DTOS.CooDtos;
using UNNew.Filters;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;
using static UNNew.DTOS.CooDtos.CooDto;
using System.Reflection;
using System.Text.Json;
using UNNew.DTOS.ContractDtos;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using UNNew.DTOS.InvoiceDto;
using UNNew.DTOS.UNEmployeeDtos;
using UNNew.DTOS.FileDto;

namespace UNNew.Repository.Services
{
    public class CooManagmentService : ICooManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CooManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        // CreateCoo
        public async Task<ApiResponse<string>> CreateCooAsync(CreateCooDto createCooDto)
        {
            try
            {
                // تحقق من وجود CooNumber في نفس السنة
                var existingCoo = _context.Coos.FirstOrDefault(x =>
                    x.CooNumber == createCooDto.CooNumber &&
                    x.CooDate.HasValue &&
                    createCooDto.CooDate.HasValue &&
                    x.CooDate.Value.Year == createCooDto.CooDate.Value.Year);

                // إذا كان يوجد Coo بنفس الرقم والسنة
                if (existingCoo != null)
                {
                    return new ApiResponse<string>(
                        "CooNumber already exists for the same year",
                        null,
                        new List<string> { "The provided CooNumber already exists for the same year. Please use a different number or year." }
                    );
                }

                // تعيين الساعة إلى 12:00 (منتصف اليوم)
                var cooDateWithMidnight = createCooDto.CooDate?.Date;


                // إنشاء الكائن Coo جديد
                var coo = new Coo
                {
                    CooNumber = createCooDto.CooNumber,
                    CooDate = cooDateWithMidnight, // تعيين القيمة المعدلة
                    TotalValue = createCooDto.TotalValue,
                    ClientId = createCooDto.ClientId,
                    CurrencyId = createCooDto.CurrencyId
                };

                // إضافة الكائن Coo إلى قاعدة البيانات
                await _context.Coos.AddAsync(coo);
                await _context.SaveChangesAsync();

                // إذا تم توفير رقم الـ Po
                if (!string.IsNullOrEmpty(createCooDto.PoNumber))
                {
                    var coopo = new CooPo
                    {
                        CooId = coo.CooId, // سيتم تعيين CooId بعد حفظ الـ Coo
                        PoNum = createCooDto.PoNumber
                    };
                    await _context.CooPos.AddAsync(coopo);
                    await _context.SaveChangesAsync();
                }

                return new ApiResponse<string>("Coo created successfully", coo.CooId.ToString(), null);
            }
            catch (Exception ex)
            {
                // في حالة حدوث أي خطأ، إرجاع رسالة خطأ
                return new ApiResponse<string>("Error occurred while creating Coo", null, new List<string> { ex.Message });
            }
        }



        // UpdateCoo
        public async Task<ApiResponse<string>> UpdateCooAsync(UpdateCooDto updateCooDto, int Id)
        {
            try
            {
                // جلب الكائن Coo من قاعدة البيانات باستخدام الـ ID
                var coo = await _context.Coos.FirstOrDefaultAsync(c => c.CooId == Id);
                if (coo == null)
                {
                    return new ApiResponse<string>("Coo not found", null, new List<string> { "Coo with the provided ID not found" });
                }

                // تحقق من وجود CooNumber في نفس السنة قبل التحديث
                if (updateCooDto.CooDate.HasValue)
                {
                    var existingCoo = await _context.Coos.FirstOrDefaultAsync(x =>
                        x.CooNumber == updateCooDto.CooNumber &&
                        x.CooDate.HasValue &&
                        x.CooDate.Value.Year == updateCooDto.CooDate.Value.Year &&
                        x.CooId != Id);  // التأكد من أن السجل ليس هو نفسه

                    if (existingCoo != null)
                    {
                        return new ApiResponse<string>(
                            "CooNumber already exists for the same year",
                            null,
                            new List<string> { "The provided CooNumber already exists for the same year. Please use a different number or year." }
                        );
                    }
                }

                // تعيين الساعة إلى 12:00 (منتصف اليوم)
                var cooDateWithMidnight = updateCooDto.CooDate?.Date;


                // تحديث البيانات الخاصة بـ Coo
                coo.CooNumber = updateCooDto.CooNumber;
                coo.CooDate = cooDateWithMidnight; // تعيين القيمة المعدلة
                coo.TotalValue = updateCooDto.TotalValue;
                coo.ClientId = updateCooDto.ClientId;
                coo.CurrencyId = updateCooDto.CurrencyId;

                // جلب CooPo المرتبط بـ Coo
                var coopo = await _context.CooPos.FirstOrDefaultAsync(x => x.CooId == Id);
                if (coopo != null)
                {
                    coopo.PoNum = updateCooDto?.PoNumber ?? null;
                }
                else
                {
                    // إذا لم يوجد CooPo، نقوم بإضافة واحد جديد
                    CooPo cooPo = new CooPo()
                    {
                        CooId = Id,
                        PoNum = updateCooDto.PoNumber
                    };
                    await _context.CooPos.AddAsync(cooPo);
                }

                // حفظ التغييرات في قاعدة البيانات
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Coo updated successfully", coo.CooId.ToString(), null);
            }
            catch (Exception ex)
            {
                // التعامل مع الخطأ بشكل مناسب مع تفاصيل إضافية
                return new ApiResponse<string>("Error occurred while updating Coo", null, new List<string> { ex.Message });
            }
        }



        // GetCooById
        public async Task<ApiResponse<GetByIdDto>> GetCooByIdAsync(int cooId)
        {
            try
            {
                var coo = await _context.Coos
                    .Include(c => c.Client)
                    .Include(c => c.Currency)
                    .Include(c => c.EmployeeCoos).ThenInclude(x=>x.Emp)
                    .Include(c => c.LifeInsurances)
                    .Include(c => c.PurchaseOrders)
                    .Include(c => c.UnEmps)
                    .Include(x=>x.CooPos)
                    .FirstOrDefaultAsync(c => c.CooId == cooId);

                if (coo == null)
                {
                    return new ApiResponse<GetByIdDto>("Coo not found", null, new List<string> { "Coo with the provided ID not found" });
                }

                var cooDto = new GetByIdDto
                {
                    CooId = coo.CooId,
                    CooNumber = coo.CooNumber,
                    CooDate = coo.CooDate,
                    TotalValue = coo.TotalValue,
                    ClientId = coo.Client?.ClientId,
                    CurrencyTypeId = coo.Currency?.Id,
                    PoNumber = coo.CooPos.FirstOrDefault(x => x.CooId == coo.CooId)?.PoNum,
                    //EmployeeCoos = coo.EmployeeCoos.Select(ec => new EmployeeCooDto { EmployeeName = ec.Emp.EmpName }).ToList(),
                };

                return new ApiResponse<GetByIdDto>("Coo fetched successfully", cooDto, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetByIdDto>("Error occurred while fetching Coo", null, new List<string> { ex.Message });
            }
        }

        // DeleteCoo
        public async Task<ApiResponse<string>> DeleteCooAsync(int cooId)
        {
            try
            {
                // البحث عن الـ Coo باستخدام الـ ID المقدم
                var coo = await _context.Coos
                    .FirstOrDefaultAsync(c => c.CooId == cooId);

                // إذا لم يتم العثور على الـ Coo، إرجاع رسالة خطأ
                if (coo == null)
                    return new ApiResponse<string>("Coo not found", null, new List<string> { "Coo with the provided ID not found" });

                // البحث عن أي علاقة مع الـ EmployeeCoos
                var relatedContract = await _context.EmployeeCoos
                    .FirstOrDefaultAsync(c => c.CooId == cooId);

                // إذا كان هناك علاقة مع العقد، لا يمكن حذف الـ Coo
                if (relatedContract != null)
                    return new ApiResponse<string>("Cannot delete this Coo because it is associated with a contract", null, new List<string> { "Coo with the provided ID is associated with a contract" });

                var CooPo=_context.CooPos.FirstOrDefault(x=>x.CooId==cooId);
                _context.Remove(CooPo);
                // حذف الـ Coo إذا لم يكن مرتبطًا بأي عقد
                _context.Coos.Remove(coo);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Coo and related data deleted successfully", null, null);
            }
            catch (Exception ex)
            {
                // إرجاع رسالة خطأ في حال حدوث استثناء
                return new ApiResponse<string>("Error occurred while deleting Coo", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<IEnumerable<CooDto>>> GetAllCoosAsync(FilterModel filterModel)
        {
            try
            {
                var query = _context.Coos.AsQueryable();
                var Count = await query.CountAsync();

                // Apply global filter
                if (!string.IsNullOrEmpty(filterModel.GlobalFilter))
                {
                    query = query.Where(coo =>
                        coo.CooNumber.Contains(filterModel.GlobalFilter) ||
                        (coo.Client != null && coo.Client.ClientName != null && coo.Client.ClientName.Contains(filterModel.GlobalFilter)) ||
                        (coo.Currency != null && coo.Currency.Type != null && coo.Currency.Type.Contains(filterModel.GlobalFilter)) ||
                        coo.TotalValue.ToString().Contains(filterModel.GlobalFilter) ||
                        coo.CooDate.ToString().Contains(filterModel.GlobalFilter));
                }

                // Apply individual filters dynamically
                query = FilterHelper.ApplyFilters(query, filterModel);

                // Pagination
                var rowsToDisplay = filterModel.Rows > 0 ? filterModel.Rows : 5;

                // Fetch data (projection to DTO)
                var coosQuery = query.Select(coo => new CooDto
                {
                    CooId = coo.CooId,
                    CooNumber = coo.CooNumber,
                    CooDate = coo.CooDate,
                    TotalValue = coo.TotalValue,
                    ClientName = coo.Client != null ? coo.Client.ClientName ?? "" : "",
                    CurrencyType = coo.Currency != null ? coo.Currency.Type ?? "" : "",
                    PoNumber = coo.CooPos.Where(x => x.CooId == coo.CooId).Select(x => x.PoNum).FirstOrDefault(),
                });

                // Fetch all data into memory
                var coos = await coosQuery.AsNoTracking().ToListAsync();

                // Additional filtering on DTO (PoNumber only)
                if (filterModel.Filters != null && filterModel.Filters.Any())
                {
                    foreach (var filter in filterModel.Filters)
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
                                        coos = coos.Where(x => x.PoNumber != null && x.PoNumber.ToLower() == value).ToList();
                                        break;
                                    case "notequals":
                                    case "not_equals":
                                        coos = coos.Where(x => x.PoNumber != null && x.PoNumber.ToLower() != value).ToList();
                                        break;
                                    case "startswith":
                                        coos = coos.Where(x => x.PoNumber != null && x.PoNumber.ToLower().StartsWith(value)).ToList();
                                        break;
                                    case "endswith":
                                        coos = coos.Where(x => x.PoNumber != null && x.PoNumber.ToLower().EndsWith(value)).ToList();
                                        break;
                                    case "contains":
                                        coos = coos.Where(x => x.PoNumber != null && x.PoNumber.ToLower().Contains(value)).ToList();
                                        break;
                                    case "notcontains":
                                        coos = coos.Where(x => x.PoNumber != null && !x.PoNumber.ToLower().Contains(value)).ToList();
                                        break;
                                }
                            }
                        }
                    }
                }

                // Multi-Sorting
                if (filterModel?.MultiSortMeta?.Any() == true)
                {
                    IOrderedQueryable<CooDto> orderedQuery = null;

                    foreach (var sort in filterModel.MultiSortMeta)
                    {
                        var field = sort.Field;
                        var sortOrder = sort.Order;

                        if (field.ToLower() == "ponumber")
                        {
                            PropertyInfo property = typeof(CooDto).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (property == null) continue;

                            var parameter = Expression.Parameter(typeof(CooDto), "entity");
                            var propertyExpression = Expression.Property(parameter, property);
                            var lambda = Expression.Lambda(propertyExpression, parameter);

                            var method = sortOrder == 1 ? "OrderBy" : "OrderByDescending";

                            var orderByExpression = Expression.Call(typeof(Queryable), method, new[] { typeof(CooDto), property.PropertyType }, coos.AsQueryable().Expression, lambda);

                            orderedQuery = orderedQuery == null
                                ? (IOrderedQueryable<CooDto>)coos.AsQueryable().Provider.CreateQuery<CooDto>(orderByExpression)
                                : (IOrderedQueryable<CooDto>)orderedQuery.Provider.CreateQuery<CooDto>(orderByExpression);
                        }
                    }

                    if (orderedQuery != null)
                        coos = orderedQuery.ToList();
                }

                // ✅ Apply pagination at the end
                coos = coos
                    .Skip(filterModel.First)
                    .Take(rowsToDisplay)
                    .ToList();

                return new ApiResponse<IEnumerable<CooDto>>("Coos fetched successfully", coos, null, Count);


                return new ApiResponse<IEnumerable<CooDto>>("Coos fetched successfully", coos, null, Count);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CooDto>>("Error occurred while fetching Coos", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<GetAllCooWithoutPaginationDto>>> GetAllCooWithoutPagination( )
        {
            try
            {

                var coo = await _context.Coos.ToListAsync();
                var Count = coo.Count;
                // إذا لم يتم العثور على الـ Coo، إرجاع رسالة خطأ
                if (coo == null)
                    return new ApiResponse<IEnumerable<GetAllCooWithoutPaginationDto>>("Coo not found", null, new List<string> { "Coo with the provided ID not found" });

                var coosQuery = coo
                      .Select(coo => new GetAllCooWithoutPaginationDto
                      {
                          CooId = coo.CooId,
                          CooNumber = coo.CooNumber,
                      });


                return new ApiResponse<IEnumerable<GetAllCooWithoutPaginationDto>>("Coos fetched successfully", coosQuery, null, Count);
            }
            catch (Exception ex)
            {
                // إرجاع رسالة خطأ في حال حدوث استثناء
                return new ApiResponse<IEnumerable<GetAllCooWithoutPaginationDto>>("Error occurred while deleting Coo", null, new List<string> { ex.Message });
            }
        }

        // You can now remove the calls to ApplyStringFilter, ApplyNumericFilter, ApplyDateFilter, etc.
        // within this method, but keep the implementations of those helper methods if they are used elsewhere.

        // GetAllCoos
        //public async Task<ApiResponse<IEnumerable<CooDto>>> GetAllCoosAsync(FilterModel filterModel)
        //{
        //    try
        //    {
        //        var query = _context.Coos.AsQueryable();
        //        var Count = query.Count();

        //        // Apply global filter
        //        if (!string.IsNullOrEmpty(filterModel.GlobalFilter))
        //        {
        //            query = query.Where(coo =>
        //                coo.CooNumber.Contains(filterModel.GlobalFilter) ||
        //                (coo.Client != null && coo.Client.ClientName.Contains(filterModel.GlobalFilter)) ||
        //                (coo.Currency != null && coo.Currency.Type.Contains(filterModel.GlobalFilter)) ||
        //                coo.TotalValue.ToString().Contains(filterModel.GlobalFilter) ||
        //                coo.CooDate.ToString().Contains(filterModel.GlobalFilter));
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
        //                        case "cooid":
        //                            query = ApplyNumericFilter(query, coo => coo.CooId, filterCriteria);
        //                            break;
        //                        case "coonumber":
        //                            query = ApplyStringFilter(query, coo => coo.CooNumber, filterCriteria);
        //                            break;
        //                        case "clientname":
        //                            query = ApplyStringFilter(query, coo => coo.Client.ClientName ?? "N/A", filterCriteria);
        //                            break;
        //                        case "currencytype":
        //                            query = ApplyStringFilter(query, coo => coo.Currency.Type ?? "N/A", filterCriteria);
        //                            break;
        //                        case "totalvalue":
        //                            query = ApplyNumericFilter(query, coo => (int?)coo.TotalValue, filterCriteria);
        //                            break;
        //                        case "coodate":
        //                            query = ApplyDateFilter(query, coo => coo.CooDate, filterCriteria);
        //                            break;
        //                        case "ponumber":
        //                            query = ApplyStringFilter(query, coo => coo.CooPos.FirstOrDefault().PoNum ?? "N/A", filterCriteria);
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
        //                var propertyInfo = typeof(Coo).GetProperties()
        //                    .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

        //                if (propertyInfo != null)
        //                {
        //                    var parameter = Expression.Parameter(typeof(Coo), "coo");
        //                    var propertyExpression = Expression.Property(parameter, propertyInfo);
        //                    var lambda = Expression.Lambda(propertyExpression, parameter);

        //                    var method = sortMeta.Order == -1 ? "OrderByDescending" : "OrderBy";
        //                    var resultExpression = Expression.Call(
        //                        typeof(Queryable),
        //                        method,
        //                        new Type[] { typeof(Coo), propertyInfo.PropertyType },
        //                        query.Expression,
        //                        lambda
        //                    );

        //                    query = query.Provider.CreateQuery<Coo>(resultExpression);
        //                }
        //            }
        //        }

        //        // Pagination
        //        var rowsToDisplay = filterModel.Rows > 0 ? filterModel.Rows : 5;

        //        var coos = await query
        //          .Skip(filterModel.First)
        //          .Take(rowsToDisplay)
        //          .Select(coo => new CooDto
        //          {
        //              CooId = coo.CooId,
        //              CooNumber = coo.CooNumber,
        //              CooDate = coo.CooDate,
        //              TotalValue = coo.TotalValue,
        //              ClientName = coo.Client != null ? coo.Client.ClientName : null,
        //              CurrencyType = coo.Currency != null ? coo.Currency.Type : null,
        //              PoNumber = coo.CooPos.Where(x => x.CooId == coo.CooId).Select(x => x.PoNum).FirstOrDefault(),

        //          })
        //          .AsNoTracking()
        //          .ToListAsync();

        //        return new ApiResponse<IEnumerable<CooDto>>("Coos fetched successfully", coos, null, Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse<IEnumerable<CooDto>>("Error occurred while fetching Coos", null, new List<string> { ex.Message });
        //    }
        //}

        //// Helper methods for filtering (same as in your original code, but adapted for Coo entity)
        //private IQueryable<Coo> ApplyStringFilter(IQueryable<Coo> query, Expression<Func<Coo, string>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    var value = filterCriteria.Value;

        //    switch (matchMode)
        //    {
        //        case "startswith":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.Call(
        //                    propertySelector.Body,
        //                    typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
        //                Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "contains":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.Call(
        //                    propertySelector.Body,
        //                    typeof(string).GetMethod("Contains", new[] { typeof(string) }),
        //                Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "endswith":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.Call(
        //                    propertySelector.Body,
        //                    typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
        //                Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(value)),
        //                propertySelector.Parameters));

        //        case "notequals": // تمت إضافة هذه الحالة
        //        case "notequal": // يمكن دعم الاسمين
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(value)),
        //                propertySelector.Parameters));
        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<Coo> ApplyNumericFilter(IQueryable<Coo> query, Expression<Func<Coo, int?>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (!int.TryParse(filterCriteria.Value, out var numericValue))
        //        return query;

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "notequals":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "lt":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.LessThan(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "lte":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.LessThanOrEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "gt":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.GreaterThan(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        case "gte":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.GreaterThanOrEqual(propertySelector.Body, Expression.Constant(numericValue)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}

        //private IQueryable<Coo> ApplyDateFilter(IQueryable<Coo> query, Expression<Func<Coo, DateTime?>> propertySelector, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    if (!DateTime.TryParse(filterCriteria.Value, out var dateValue))
        //        return query;

        //    switch (matchMode)
        //    {
        //        case "equals":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.Equal(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        case "notequals":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.NotEqual(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        case "before":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.LessThan(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        case "after":
        //            return query.Where(Expression.Lambda<Func<Coo, bool>>(
        //                Expression.GreaterThan(propertySelector.Body, Expression.Constant(dateValue)),
        //                propertySelector.Parameters));

        //        default:
        //            return query;
        //    }
        //}
        public async Task<ApiResponse<IEnumerable<FileResponseDto>>> UploadFilesForCooAsync(int cooId, List<IFormFile> files)
        {
            try
            {
                var coo = await _context.Coos.FindAsync(cooId);
                if (coo == null)
                {
                    return new ApiResponse<IEnumerable<FileResponseDto>>("Coo not found", null, new List<string> { "The Coo with the specified ID does not exist." });
                }

                if (files == null || !files.Any())
                {
                    return new ApiResponse<IEnumerable<FileResponseDto>>("No files uploaded", null, new List<string> { "Please upload at least one file." });
                }

                string contentRootPath = _configuration["FileStorage:FilesCoosPath"];
                var cooFolderPath = Path.Combine(contentRootPath, coo.CooNumber.ToString());

                if (!Directory.Exists(cooFolderPath))
                {
                    Directory.CreateDirectory(cooFolderPath);
                }

                var uploadedFiles = new List<FileResponseDto>();
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var originalName = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "_");
                        var extension = Path.GetExtension(file.FileName);

                        // التحقق من وجود اسم مكرر (نفس الاسم الأساسي بغض النظر عن التاريخ والامتداد)
                        bool isDuplicate = Directory.GetFiles(cooFolderPath)
                            .Select(f => Path.GetFileNameWithoutExtension(f).Split('_')[0])
                            .Any(f => string.Equals(f, originalName, StringComparison.OrdinalIgnoreCase));

                        if (isDuplicate)
                        {
                            return new ApiResponse<IEnumerable<FileResponseDto>>("Duplicate file name", null,
                                new List<string> { $"A file with the name '{originalName}' already exists for this Coo." });
                        }

                        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                        var uniqueFileName = $"{originalName}_{timestamp}{extension}";
                        var filePath = Path.Combine(cooFolderPath, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        uploadedFiles.Add(new FileResponseDto { FileName = uniqueFileName });

                        if (string.IsNullOrEmpty(coo.FolderPath))
                        {
                            coo.FolderPath = coo.CooNumber.ToString();
                            _context.Coos.Update(coo);
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

        public async Task<ApiResponse<IEnumerable<FileResponseDto>>> GetCooFilesAsync(int cooId)
        {
            try
            {
                var coo = await _context.Coos.FindAsync(cooId);
                if (coo == null)
                {
                    return new ApiResponse<IEnumerable<FileResponseDto>>("Coo not found", null, new List<string> { "The Coo with the specified ID does not exist." });
                }

                string contentRootPath = _configuration["FileStorage:FilesCoosPath"];
                var cooFolderPath = Path.Combine(contentRootPath, coo.CooNumber.ToString());

                if (!Directory.Exists(cooFolderPath))
                {
                    return new ApiResponse<IEnumerable<FileResponseDto>>("No files found", null, new List<string> { "The Coo does not have any uploaded files." });
                }

                var files = Directory.GetFiles(cooFolderPath)
                    .Select(file => new FileResponseDto { FileName = Path.GetFileName(file) })
                    .ToList();

                return new ApiResponse<IEnumerable<FileResponseDto>>("Files retrieved successfully", files, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<FileResponseDto>>("Failed to retrieve files", null, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<(byte[] content, string contentType, string fileName)>> DownloadCooFileAsync(int cooId, string fileName)
        {
            try
            {
                var coo = await _context.Coos.FindAsync(cooId);
                if (coo == null)
                {
                    return new ApiResponse<(byte[] content, string contentType, string fileName)>("Coo not found", (null, null, null), new List<string> { "The Coo with the specified ID does not exist." });
                }

                string contentRootPath = _configuration["FileStorage:FilesCoosPath"];
                var cooFolderPath = Path.Combine(contentRootPath, coo.CooNumber.ToString());
                var filePath = Path.Combine(cooFolderPath, fileName); // إضافة اسم الملف إلى المسار

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

        public async Task<ApiResponse<bool>> DeleteCooFileAsync(int cooId, string fileName)
        {
            try
            {
                var coo = await _context.Coos.FindAsync(cooId);
                if (coo == null)
                {
                    return new ApiResponse<bool>("Coo not found", false, new List<string> { "The Coo with the specified ID does not exist." });
                }

                string contentRootPath = _configuration["FileStorage:FilesCoosPath"];
                var cooFolderPath = Path.Combine(contentRootPath, coo.CooNumber.ToString());
                var filePath = Path.Combine(cooFolderPath, fileName); // إضافة اسم الملف إلى المسار

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

        public async Task<ApiResponse<bool>> DeleteAllCooFilesAsync(int cooId)
        {
            try
            {
                var coo = await _context.Coos.FindAsync(cooId);
                if (coo == null)
                {
                    return new ApiResponse<bool>("Coo not found", false, new List<string> { "The Coo with the specified ID does not exist." });
                }

                string contentRootPath = _configuration["FileStorage:FilesCoosPath"];
                var cooFolderPath = Path.Combine(contentRootPath, coo.CooNumber.ToString());

                if (!Directory.Exists(cooFolderPath))
                {
                    return new ApiResponse<bool>("No files found", false, new List<string> { "The Coo does not have any uploaded files." });
                }

                try
                {
                    var files = Directory.GetFiles(cooFolderPath);
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
