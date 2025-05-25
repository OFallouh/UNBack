using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Linq;
using UNNew.DTOS.ClientDtos;
using UNNew.DTOS.ContractDtos;
using UNNew.DTOS.CooDtos;
using UNNew.Filters;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;
using static CreateUNEmployeeDto;
using System.Text.Json;
using System.Reflection;
using UNNew.Helpers;
using UNNew.DTOS.PoNumberDto;

namespace UNNew.Repository.Services
{
    public class ContractManagmentService : IContractManagmentService
    { 
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ContractManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponse<GetByIdContractDto>> GetContractByIdAsync(int ContractId)
        {
            try
            {
                var contract = await _context.EmployeeCoos.Include(x=>x.Client)
                    .Include(x=>x.Team).Include(x=>x.Coo).Include(x => x.City)
                    .Include(x => x.TypeOfContract)
                    .Include(x => x.LaptopNavigation).FirstOrDefaultAsync(c => c.Id == ContractId);
                if (contract == null)
                {
                    return new ApiResponse<GetByIdContractDto>("Contract not found", null, new List<string> { "Contract with the provided ID not found" });
                }

                var ContracttDto = new GetByIdContractDto
                {
                    Id = contract.Id,
                    ClientId = contract?.Client?.ClientId,
                    TeamId = contract?.Team?.TeamId ,
                    CooNumber = contract?.Coo?.CooNumber,              
                    ContractStartDate = contract?.StartCont,
                    ContractEndDate = contract?.EndCont,
                    CityId = contract?.City?.CityId ,
                    Tittle = contract?.Tittle ?? "",
                    Salary = contract?.Salary ?? 0,
                    Transportation = contract?.Transportation,
                    LaptopTypeId = contract?.LaptopNavigation?.Id,
                    IsMobile = contract?.IsMobile ?? false,
                    TypeOfContractId = contract?.TypeOfContract?.TypeOfContractId ,
                    SuperVisor = contract?.SuperVisor ?? "",
                    AreaManager = contract?.AreaManager ?? "",
                    ProjectName = contract?.ProjectName ?? "",
                    CooId = contract?.Coo?.CooId,
                    InsuranceLife = contract?.InsuranceLife,
                    InsuranceMedical = contract?.InsuranceMedical,
                    

                };

                return new ApiResponse<GetByIdContractDto>("Contract fetched successfully", ContracttDto, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetByIdContractDto>("Error occurred while fetching Contract", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<GetByIdContractDto>> GetContractByEmployeeIdAsync(int Id)
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
                var contract = await _context.EmployeeCoos.Include(x => x.Client)
                    .Include(x => x.Team).Include(x => x.Coo).Include(x => x.City)
                    .Include(x => x.TypeOfContract)
                    .Include(x => x.LaptopNavigation).FirstOrDefaultAsync(c => c.EmpId == Id && c.EndCont >= today);
                if (contract == null)
                {
                    return new ApiResponse<GetByIdContractDto>("No active contract found.", null, new List<string> { "No active contract found for this employee." });

                }

                var ContracttDto = new GetByIdContractDto
                {
                    Id = contract.Id,
                    ClientId = contract?.Client?.ClientId,
                    TeamId = contract?.Team?.TeamId,
                    CooNumber = contract?.Coo?.CooNumber,
                    ContractStartDate = contract?.StartCont,
                    ContractEndDate = contract?.EndCont,
                    CityId = contract?.City?.CityId,
                    Tittle = contract?.Tittle ?? "",
                    Salary = contract?.Salary ?? 0,
                    Transportation = contract?.Transportation,
                    LaptopTypeId = contract?.LaptopNavigation?.Id,
                    IsMobile = contract?.IsMobile ?? false,
                    TypeOfContractId = contract?.TypeOfContract?.TypeOfContractId,
                    SuperVisor = contract?.SuperVisor ?? "",
                    AreaManager = contract?.AreaManager ?? "",
                    ProjectName = contract?.ProjectName ?? "",
                    CooId = contract?.Coo?.CooId,
                    InsuranceLife = contract?.InsuranceLife,
                    InsuranceMedical = contract?.InsuranceMedical,


                };

                return new ApiResponse<GetByIdContractDto>("Contract fetched successfully", ContracttDto, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetByIdContractDto>("Error occurred while fetching Contract", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<string>> CreateContract(AddContractDto addContractInfo)
        {
            try
            {
                int? ClientId = null;

                // Convert the nullable DateTime to DateOnly
                var contractStartDate = addContractInfo.ContractStartDate.HasValue
                    ? DateOnly.FromDateTime(addContractInfo.ContractStartDate.Value)
                    : (DateOnly?)null;

                // Check for existing active contract
                var existingContract = await _context.EmployeeCoos
                    .FirstOrDefaultAsync(x => x.EmpId == addContractInfo.EmployeeId
                                           && x.EndCont >= contractStartDate
                                           && x.IsCancelled == false);

                if (existingContract != null)
                {
                    return new ApiResponse<string>(
                        "This employee already has an active contract that ends after the new contract start date.",
                        null,
                        new List<string> { "Active contract already exists for this employee." });
                }

                // Validate start and end date
                if (addContractInfo.ContractStartDate > addContractInfo.ContractEndDate)
                {
                    return new ApiResponse<string>(
                        "Contract start date cannot be after the end date.",
                        null,
                        new List<string> { "Start date must be before or equal to the end date." });
                }

                var coo = await _context.Coos.FirstOrDefaultAsync(x => x.CooId == addContractInfo.CooId);
                if (coo == null)
                {
                    return new ApiResponse<string>(
                        "COO not found.",
                        null,
                        new List<string> { "The specified COO ID does not exist." });
                }

                // Calculate total salary for this COO
                var totalCurrentSalary = await _context.EmployeeCoos
                    .Where(x => x.CooId == addContractInfo.CooId && x.IsCancelled == false)
                    .SumAsync(x => (int?)x.Salary) ?? 0;

                var newTotal = totalCurrentSalary + addContractInfo.Salary;

                if (newTotal > coo.TotalValue)
                {
                    return new ApiResponse<string>(
                        "Total salaries exceed the allowed limit for this COO.",
                        null,
                        new List<string> { $"Maximum allowed: {coo.TotalValue}, current after adding: {newTotal}" });
                }

                // Get client ID from team
                var client = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == addContractInfo.TeamId);
                if (client != null)
                {
                    ClientId = client.ClientId;
                }

                // Map and add new contract
                EmployeeCoo? newContract = _mapper.Map<EmployeeCoo>(addContractInfo);
                newContract.IsCancelled = false;
                newContract.ClientId = ClientId;

                _context.EmployeeCoos.Add(newContract);

                // Update employee info
                var employee = await _context.UnEmps.FindAsync(addContractInfo.EmployeeId);
                if (employee != null)
                {
                    employee.Active = true;
                    employee.InsuranceLife = addContractInfo.InsuranceLife;
                    employee.InsuranceMedical = addContractInfo.InsuranceMedical;
                }

                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Contract created successfully.", newContract.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    "An error occurred while creating the contract.",
                    null,
                    new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<string>> UpdateContractAsync(int Id, UpdateContractDto updateContractDto)
        {
            try
            {
                // التأكد من وجود العقد
                EmployeeCoo? Contract = await _context.EmployeeCoos.Include(x => x.Emp).FirstOrDefaultAsync(x => x.Id == Id);
                if (Contract == null)
                {
                    return new ApiResponse<string>("Contract not found", null, new List<string> { "Contract with the provided ID not found" });
                }

                // ✅ تحقق من تاريخ البداية والنهاية
                if (updateContractDto.ContractStartDate > updateContractDto.ContractEndDate)
                {
                    return new ApiResponse<string>(
                        "Contract start date cannot be after end date.",
                        null,
                        new List<string> { "Start date must be before or equal to end date." });
                }

                // التأكد من وجود العميل
                var client = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == Contract.TeamId);
                if (client != null)
                {
                    Contract.ClientId = client.ClientId; // تعيين ClientId فقط إذا كان العميل موجودًا
                }

                // التحقق من وجود Emp قبل التحديث
                if (Contract.Emp != null)
                {
                    Contract.Emp.InsuranceLife = updateContractDto.InsuranceLife;
                    Contract.Emp.InsuranceMedical = updateContractDto.InsuranceMedical;
                }

                // تحديث العقد باستخدام الـ Mapper
                _mapper.Map(updateContractDto, Contract);

                // حفظ التعديلات
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Contract updated successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while updating the Contract", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<string>> DeleteContractAsync(int id)
        {
            try
            {
                var Contract = await _context.EmployeeCoos.FindAsync(id);
                if (Contract == null)
                {
                    return new ApiResponse<string>("Contract not found", null, new List<string> { "Contract with the provided ID not found" });
                }

                _context.EmployeeCoos.Remove(Contract);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Contract deleted successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while deleting the Contract", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<string>> UpdateEndContractBeforCancel (int Id,BeforCancelContractDto beforCancelContractDto)
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
                var Contract = await _context.EmployeeCoos.Include(x => x.Emp).FirstOrDefaultAsync(x => x.Id == Id);
                if (Contract == null && Contract.EndCont >= today)
                {
                    return new ApiResponse<string>("Contract not found or not active", null, new List<string> { "Contract with the provided ID not found or not active" });
                }
                Contract.EndCont = beforCancelContractDto.ContractEndDate;

                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Contract deleted successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while deleting the Contract", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<string>> CancelContractAsync(int id, BeforCancelContractDto beforCancelContractDto)
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
                var Contract = await _context.EmployeeCoos.Include(x => x.Emp).FirstOrDefaultAsync(x=>x.Id==id);
                if (Contract == null && Contract.EndCont>= today)
                {
                    return new ApiResponse<string>("Contract not found or not active", null, new List<string> { "Contract with the provided ID not found or not active" });
                }
                 Contract.IsCancelled = true;
                 Contract.EndCont = beforCancelContractDto.ContractEndDate;
                 Contract.Emp.Active =false;
                 
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Contract deleted successfully", null, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while deleting the Contract", null, new List<string> { ex.Message });
            }
        }

        //public async Task<ApiResponse<IEnumerable<ContractDto>>> GetAllContractsAsync(FilterModel filterModel, int? Id)
        //{
        //    try
        //    {
        //        var query = _context.EmployeeCoos
        //            .AsNoTracking()
        //            .Include(c => c.Client)
        //            .Include(c => c.Team)
        //            .Include(c => c.City)
        //            .Include(c => c.Coo)
        //            .Include(c => c.Emp)
        //            .Include(c => c.LaptopNavigation)
        //            .Include(c => c.TypeOfContract)
        //            .AsQueryable();

        //        // Filter by employee ID if provided
        //        if (Id.HasValue)
        //        {
        //            query = query.Where(x => x.EmpId == Id.Value);
        //        }

        //        var Count = await query.CountAsync();

        //        // Apply global filter
        //        if (!string.IsNullOrEmpty(filterModel.GlobalFilter))
        //        {
        //            query = query.Where(contract =>
        //                (contract.Client != null && contract.Client.ClientName.Contains(filterModel.GlobalFilter)) ||
        //                (contract.Team != null && contract.Team.TeamName.Contains(filterModel.GlobalFilter)) ||
        //                (contract.Coo != null && contract.Coo.CooNumber.Contains(filterModel.GlobalFilter)) ||
        //                (contract.City != null && contract.City.NameEn.Contains(filterModel.GlobalFilter)) ||
        //                (contract.Emp != null && contract.Emp.EmpName.Contains(filterModel.GlobalFilter)) ||
        //                (contract.TypeOfContract != null && contract.TypeOfContract.NmaeEn.Contains(filterModel.GlobalFilter)) ||
        //                (contract.Tittle != null && contract.Tittle.Contains(filterModel.GlobalFilter)) ||
        //                (contract.SuperVisor != null && contract.SuperVisor.Contains(filterModel.GlobalFilter)) ||
        //                (contract.AreaManager != null && contract.AreaManager.Contains(filterModel.GlobalFilter)) ||
        //                (contract.ProjectName != null && contract.ProjectName.Contains(filterModel.GlobalFilter)));
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
        //                        case "employeename":
        //                            query = ApplyStringFilter(query, c => c.Emp != null ? c.Emp.EmpName : "", filterCriteria);
        //                            break;
        //                        case "arabicname":
        //                            query = ApplyStringFilter(query, c => c.Emp != null ? c.Emp.ArabicName : "", filterCriteria);
        //                            break;
        //                        case "clientname":
        //                            query = ApplyStringFilter(query, c => c.Client != null ? c.Client.ClientName : "", filterCriteria);
        //                            break;
        //                        case "teamname":
        //                            query = ApplyStringFilter(query, c => c.Team != null ? c.Team.TeamName : "", filterCriteria);
        //                            break;
        //                        case "cooid":
        //                            query = ApplyNumericFilter(query, c => c.CooId, filterCriteria);
        //                            break;
        //                        case "contractsigned":
        //                            query = ApplyBooleanFilter(query, c => c.ContractSigned, filterCriteria);
        //                            break;
        //                        case "contractstartdate":
        //                            query = ApplyDateFilter(query, c => c.StartCont, filterCriteria);
        //                            break;
        //                        case "contractenddate":
        //                            query = ApplyDateFilter(query, c => c.EndCont, filterCriteria);
        //                            break;
        //                        case "cityname":
        //                            query = ApplyStringFilter(query, c => c.City != null ? c.City.NameEn : "", filterCriteria);
        //                            break;
        //                        case "tittle":
        //                            query = ApplyStringFilter(query, c => c.Tittle, filterCriteria);
        //                            break;
        //                        case "salary":
        //                            query = ApplyNumericFilter(query, c => c.Salary, filterCriteria);
        //                            break;
        //                        case "transportation":
        //                            query = ApplyBooleanFilter(query, c => c.Transportation, filterCriteria);
        //                            break;
        //                        case "laptop":
        //                            query = ApplyStringFilter(query, c => c.Laptop != null ? c.LaptopNavigation.Name : "", filterCriteria);
        //                            break;
        //                        case "ismobile":
        //                            query = ApplyBooleanFilter(query, c => c.IsMobile, filterCriteria);
        //                            break;
        //                        case "iscancelled":
        //                            query = ApplyBooleanFilter(query, c => c.IsCancelled, filterCriteria);
        //                            break;
        //                        case "typeofcontract":
        //                            query = ApplyStringFilter(query, c => c.TypeOfContract != null ? c.TypeOfContract.NmaeEn : "", filterCriteria);
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
        //                        case "supervisor":
        //                            query = ApplyStringFilter(query, c => c.SuperVisor, filterCriteria);
        //                            break;
        //                        case "areamanager":
        //                            query = ApplyStringFilter(query, c => c.AreaManager, filterCriteria);
        //                            break;
        //                        case "projectname":
        //                            query = ApplyStringFilter(query, c => c.ProjectName, filterCriteria);
        //                            break;
        //                        case "status":
        //                            query = ApplyStatusFilter(query, filterCriteria);
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
        //                    var parameter = Expression.Parameter(typeof(EmployeeCoo), "contract");
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

        //        var contracts = await query
        //            .Skip(filterModel.First)
        //            .Take(rowsToDisplay)
        //            .ToListAsync();

        //        var contractDtos = new List<ContractDto>();

        //        foreach (var contract in contracts)
        //        {
        //            try
        //            {
        //                var cooPoId = await _context.CooPos
        //                    .AsNoTracking()
        //                    .Where(x => x.CooId == contract.CooId)
        //                    .Select(x => x.PoNum)
        //                    .FirstOrDefaultAsync();

        //                contractDtos.Add(new ContractDto
        //                {
        //                    Id = contract.Id,
        //                    EmployeeName = contract.Emp != null ? contract.Emp.EmpName : null,
        //                    ArabicName = contract.Emp != null ? contract.Emp.ArabicName : null,
        //                    ClientName = contract.Client != null ? contract.Client.ClientName : null,
        //                    TeamName = contract.Team != null ? contract.Team.TeamName : null,
        //                    CooId = contract.CooId,
        //                    CooPoId = cooPoId,
        //                    ContractSigned = contract.ContractSigned,
        //                    ContractStartDate = contract.StartCont,
        //                    ContractEndDate = contract.EndCont,
        //                    CityName = contract.City != null ? contract.City.NameEn : null,
        //                    Tittle = contract.Tittle,
        //                    Salary = contract.Salary,
        //                    Transportation = contract.Transportation,
        //                    LaptopType = contract.LaptopNavigation.Name,
        //                    IsMobile = contract.IsMobile,
        //                    TypeOfContract = contract.TypeOfContract != null ? contract.TypeOfContract.NmaeEn : null,
        //                    InsuranceLife = contract.InsuranceLife,
        //                    StartLifeDate = contract.StartLifeDate,
        //                    EndLifeDate = contract.EndLifeDate,
        //                    InsuranceMedical = contract.InsuranceMedical,
        //                    SuperVisor = contract.SuperVisor,
        //                    AreaManager = contract.AreaManager,
        //                    ProjectName = contract.ProjectName,
        //                    Status = GetContractStatus(contract.ContractSigned, contract.StartCont, contract.EndCont, contract.IsCancelled),
        //                    ContractDuration = CalculateContractDuration(contract.StartCont, contract.EndCont)
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Error processing contract ID {contract.Id}: {ex.Message}");
        //            }
        //        }

        //        return new ApiResponse<IEnumerable<ContractDto>>("Contracts fetched successfully", contractDtos, null, Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse<IEnumerable<ContractDto>>(
        //            "Error occurred while fetching Contracts", null, new List<string> { ex.Message });
        //    }
        //}

        //// Helper methods for filtering (similar to previous implementations)
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

        //private string GetContractStatus(bool? contractSigned, DateOnly? startDate, DateOnly? endDate, bool? isCancelled)
        //{
        //    if (isCancelled == true)
        //        return "Cancelled";

        //    if (contractSigned == true)
        //    {
        //        if (startDate.HasValue && endDate.HasValue)
        //        {
        //            var today = DateOnly.FromDateTime(DateTime.Today); 

        //            if (today < startDate.Value)
        //                return "Not started yet";
        //            else if (today >= startDate.Value && today <= endDate.Value)
        //                return "Active";
        //            else
        //                return "Expired";
        //        }
        //    }

        //    return "Undefined";
        //}

        //public string CalculateContractDuration(DateOnly? startDate, DateOnly? endDate)
        //{
        //    if (startDate == null || endDate == null)
        //    {
        //        return "Invalid Dates";
        //    }

        //    var start = startDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert DateOnly to DateTime
        //    var end = endDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert DateOnly to DateTime
        //    var duration = end - start; // Calculate the difference between the two dates

        //    if (duration.Days < 0)
        //    {
        //        return "Invalid Duration"; // If the end date is before the start date
        //    }

        //    // Calculate years, months, and days
        //    int years = duration.Days / 365;
        //    int months = (duration.Days % 365) / 30;
        //    int days = (duration.Days % 365) % 30;

        //    if (years > 0)
        //    {
        //        return $"{years} year" + (years > 1 ? "s" : "");
        //    }
        //    if (months > 0)
        //    {
        //        return $"{months} month" + (months > 1 ? "s" : "");
        //    }
        //    return $"{days} day" + (days > 1 ? "s" : "");
        //}
        //private IQueryable<EmployeeCoo> ApplyStatusFilter(IQueryable<EmployeeCoo> query, FilterCriteria filterCriteria)
        //{
        //    var matchMode = filterCriteria.MatchMode?.ToLower();
        //    var value = filterCriteria.Value?.ToLower();
        //    var today = DateOnly.FromDateTime(DateTime.Today);

        //    return query.Where(contract =>
        //        (matchMode == "equals" &&
        //         GetContractStatus(contract.ContractSigned, contract.StartCont, contract.EndCont, contract.IsCancelled).ToLower() == value) ||
        //        (matchMode == "notequals" &&
        //         GetContractStatus(contract.ContractSigned, contract.StartCont, contract.EndCont, contract.IsCancelled).ToLower() != value) ||
        //        (matchMode == "contains" &&
        //         GetContractStatus(contract.ContractSigned, contract.StartCont, contract.EndCont, contract.IsCancelled).ToLower().Contains(value)) ||
        //        (matchMode == "startswith" &&
        //         GetContractStatus(contract.ContractSigned, contract.StartCont, contract.EndCont, contract.IsCancelled).ToLower().StartsWith(value)) ||
        //        (matchMode == "endswith" &&
        //         GetContractStatus(contract.ContractSigned, contract.StartCont, contract.EndCont, contract.IsCancelled).ToLower().EndsWith(value)) ||
        //        string.IsNullOrEmpty(matchMode)
        //    );
        //}
        public async Task<ApiResponse<IEnumerable<ContractDto>>> GetAllContractsAsync(FilterModel filterRequest, int? Id)
        {
            if (filterRequest == null)
            {
                return new ApiResponse<IEnumerable<ContractDto>>(
                    "Error: Filter model cannot be null",
                    null,
                    new List<string> { "Filter model is null" },
                    0
                );
            }

            try
            {
                int totalCount = 0;

                var query = _context.EmployeeCoos
                    .AsNoTracking()
                    .Include(c => c.Client)
                    .Include(c => c.Team)
                    .Include(c => c.City)
                    .Include(c => c.Coo)
                    .Include(c => c.Emp)
                    .Include(c => c.LaptopNavigation)
                    .Include(c => c.TypeOfContract)
                    .AsQueryable();

                if (Id.HasValue)
                {
                    query = query.Where(x => x.EmpId == Id.Value);
                }
                totalCount = await query.CountAsync(); 
                if (!string.IsNullOrEmpty(filterRequest.GlobalFilter))
                {
                    var searchTerm = filterRequest.GlobalFilter.ToLower();

                    query = query.Where(contract =>
                        (contract.Emp != null && contract.Emp.EmpName.ToLower().Contains(searchTerm)) ||
                        (contract.Emp != null && contract.Emp.ArabicName.ToLower().Contains(searchTerm)) ||
                        (contract.Client != null && contract.Client.ClientName.ToLower().Contains(searchTerm)) ||
                        (contract.Team != null && contract.Team.TeamName.ToLower().Contains(searchTerm)) ||
                        (contract.City != null && contract.City.NameEn.ToLower().Contains(searchTerm)) ||
                        (contract.Coo != null && contract.Coo.CooNumber.ToLower().Contains(searchTerm)) ||
                        contract.Tittle.ToLower().Contains(searchTerm) ||
                        contract.Salary.ToString().Contains(searchTerm) ||
                        contract.Transportation.ToString().Contains(searchTerm) ||
                        (contract.LaptopNavigation != null && contract.LaptopNavigation.Name.ToLower().Contains(searchTerm)) ||
                        (contract.TypeOfContract != null && contract.TypeOfContract.NmaeEn.ToLower().Contains(searchTerm)) ||
                        contract.SuperVisor.ToLower().Contains(searchTerm) ||
                        contract.AreaManager.ToLower().Contains(searchTerm) ||
                        contract.ProjectName.ToLower().Contains(searchTerm)
                    );
                }

                // Apply other filters
                query = FilterHelper.ApplyFilters(query, filterRequest);

                var contracts = await query.ToListAsync();

                var contractDtos = new List<ContractDto>();

                foreach (var contract in contracts)
                {
                    try
                    {
                        var cooPoId = await _context.CooPos
                            .AsNoTracking()
                            .Where(x => x.CooId == contract.CooId)
                            .Select(x => x.PoNum)
                            .FirstOrDefaultAsync();

                        contractDtos.Add(new ContractDto
                        {
                            Id = contract.Id,
                            EmployeeName = contract.Emp?.EmpName,
                            ArabicName = contract.Emp?.ArabicName,
                            ClientName = contract.Client?.ClientName,
                            TeamName = contract.Team?.TeamName,
                            CooNumber = contract?.Coo?.CooNumber,
                            ContractStartDate = contract?.StartCont,
                            ContractEndDate = contract?.EndCont,
                            CityName = contract?.City?.NameEn,
                            Tittle = contract?.Tittle,
                            Salary = contract?.Salary,
                            Transportation = contract?.Transportation,
                            LaptopType = contract?.LaptopNavigation?.Name,
                            IsMobile = contract?.IsMobile,
                            TypeOfContract = contract?.TypeOfContract?.NmaeEn,
                            InsuranceLife = contract?.InsuranceLife,
                            StartLifeDate = contract?.StartLifeDate,
                            EndLifeDate = contract?.EndLifeDate,
                            InsuranceMedical = contract?.InsuranceMedical,
                            SuperVisor = contract?.SuperVisor,
                            AreaManager = contract?.AreaManager,
                            ProjectName = contract?.ProjectName,
                            Status = GetContractStatus(contract?.ContractSigned, contract?.StartCont, contract?.EndCont, contract?.IsCancelled),
                            ContractDuration = CalculateContractDuration(contract?.StartCont, contract?.EndCont)
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing contract ID {contract.Id}: {ex.Message}");
                    }
                }

                // Apply additional filters on DTOs
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

                            if (filterValue.TryGetProperty("MatchMode", out JsonElement matchModeElement))
                            {
                                string matchMode = matchModeElement.GetString();

                                if (filter.Key.ToLower() == "status")
                                {
                                    string value = valueElement.GetString().ToLower();

                                    switch (matchMode.ToLower())
                                    {
                                        case "equals":
                                            contractDtos = contractDtos.Where(x => x.Status != null && x.Status.ToLower() == value).ToList();
                                            break;
                                        case "notequals":
                                        case "not_equals":
                                            contractDtos = contractDtos.Where(x => x.Status != null && x.Status.ToLower() != value).ToList();
                                            break;
                                        case "startswith":
                                            contractDtos = contractDtos.Where(x => x.Status != null && x.Status.ToLower().StartsWith(value)).ToList();
                                            break;
                                        case "endswith":
                                            contractDtos = contractDtos.Where(x => x.Status != null && x.Status.ToLower().EndsWith(value)).ToList();
                                            break;
                                        case "contains":
                                            contractDtos = contractDtos.Where(x => x.Status != null && x.Status.ToLower().Contains(value)).ToList();
                                            break;
                                        case "notcontains":
                                            contractDtos = contractDtos.Where(x => x.Status != null && !x.Status.ToLower().Contains(value)).ToList();
                                            break;
                                    }
                                }
                                else if (filter.Key.ToLower() == "contractduration")
                                {
                                    string value = valueElement.GetString().ToLower();

                                    switch (matchMode.ToLower())
                                    {
                                        case "equals":
                                            contractDtos = contractDtos.Where(x => x.ContractDuration != null && x.ContractDuration.ToLower() == value).ToList();
                                            break;
                                        case "notequals":
                                        case "not_equals":
                                            contractDtos = contractDtos.Where(x => x.ContractDuration != null && x.ContractDuration.ToLower() != value).ToList();
                                            break;
                                        case "startswith":
                                            contractDtos = contractDtos.Where(x => x.ContractDuration != null && x.ContractDuration.ToLower().StartsWith(value)).ToList();
                                            break;
                                        case "endswith":
                                            contractDtos = contractDtos.Where(x => x.ContractDuration != null && x.ContractDuration.ToLower().EndsWith(value)).ToList();
                                            break;
                                        case "contains":
                                            contractDtos = contractDtos.Where(x => x.ContractDuration != null && x.ContractDuration.ToLower().Contains(value)).ToList();
                                            break;
                                        case "notcontains":
                                            contractDtos = contractDtos.Where(x => x.ContractDuration != null && !x.ContractDuration.ToLower().Contains(value)).ToList();
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Apply sorting
                if (filterRequest?.MultiSortMeta?.Any() == true)
                {
                    IOrderedQueryable<ContractDto> orderedQuery = null;

                    foreach (var sort in filterRequest.MultiSortMeta)
                    {
                        var field = sort.Field;
                        var sortOrder = sort.Order;

                        if (field.ToLower() == "status" || field.ToLower() == "contractduration" || field.ToLower() == "arabicname" || field.ToLower() == "employeename")
                        {
                            PropertyInfo property = typeof(ContractDto).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (property == null) continue;

                            var parameter = Expression.Parameter(typeof(ContractDto), "entity");
                            var propertyExpression = Expression.Property(parameter, property);
                            var lambda = Expression.Lambda(propertyExpression, parameter);

                            var method = sortOrder == 1 ? "OrderBy" : "OrderByDescending";

                            var orderByExpression = Expression.Call(typeof(Queryable), method, new[] { typeof(ContractDto), property.PropertyType }, contractDtos.AsQueryable().Expression, lambda);

                            orderedQuery = orderedQuery == null
                                ? (IOrderedQueryable<ContractDto>)contractDtos.AsQueryable().Provider.CreateQuery<ContractDto>(orderByExpression)
                                : (IOrderedQueryable<ContractDto>)orderedQuery.Provider.CreateQuery<ContractDto>(orderByExpression);
                        }
                    }

                    contractDtos = orderedQuery?.ToList() ?? contractDtos;
                }

                // Apply pagination at the very end
                contractDtos = contractDtos
                    .Skip(filterRequest.First)
                    .Take(filterRequest.Rows > 0 ? filterRequest.Rows : 5)
                    .ToList();

                return new ApiResponse<IEnumerable<ContractDto>>("Contracts fetched successfully", contractDtos, null, totalCount);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ContractDto>>(
                    "Error fetching contracts",
                    null,
                    new List<string> { ex.Message },
                    0
                );
            }
        }

        private string GetContractStatus(bool? contractSigned, DateOnly? startDate, DateOnly? endDate, bool? isCancelled)
        {
            if (isCancelled == true)
                return "Cancelled";

        
            if (startDate.HasValue && endDate.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                if (today < startDate.Value)
                    return "Not started yet";
                else if (today >= startDate.Value && today <= endDate.Value)
                    return "Active";
                else
                    return "Expired";
            }
            

            return "Undefined";
        }

        public string CalculateContractDuration(DateOnly? startDate, DateOnly? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return "Invalid Dates";
            }

            var start = startDate.Value.ToDateTime(new TimeOnly(0, 0));
            var end = endDate.Value.ToDateTime(new TimeOnly(0, 0));
            var duration = end - start;

            if (duration.Days < 0)
            {
                return "Invalid Duration";
            }

            int years = duration.Days / 365;
            int months = (duration.Days % 365) / 30;
            int days = (duration.Days % 365) % 30;

            if (years > 0)
            {
                return $"{years} year" + (years > 1 ? "s" : "");
            }
            if (months > 0)
            {
                return $"{months} month" + (months > 1 ? "s" : "");
            }
            return $"{days} day" + (days > 1 ? "s" : "");
        }
        public async Task<ApiResponse<List<GetAllPoNumberDto>>> GetAllPoNumber()
        {
            try
            {
                var poNumbers = await _context.CooPos
                    .Where(x => x.Coo != null && x.Coo.Client != null) // الحماية من null
                    .Select(x => new GetAllPoNumberDto
                    {
                        PoNumber = x.PoNum,
                        ClientName = x.Coo.Client.ClientName
                    })
                    .ToListAsync();

                return new ApiResponse<List<GetAllPoNumberDto>>("Data retrieved successfully", poNumbers, null);
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ إن كنت تستخدم NLog أو أي Logger آخر
                // _logger.LogError(ex, "Error in GetAllPoNumber");

                return new ApiResponse<List<GetAllPoNumberDto>>(
                    "Error occurred while retrieving the data",
                    null,
                    new List<string> { ex.Message, ex.InnerException?.Message }
                        .Where(msg => !string.IsNullOrEmpty(msg))
                        .ToList()
                );
            }
        }


    }
}
