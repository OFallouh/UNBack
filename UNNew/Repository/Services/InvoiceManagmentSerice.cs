using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using UNNew.DTOS.ContractDtos;
using UNNew.DTOS.InvoiceDto;
using UNNew.DTOS.SalaryDtos;
using UNNew.Filters;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Response;

namespace UNNew.Repository.Services
{
    public class InvoiceManagmentSerice: IInvoiceManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public InvoiceManagmentSerice(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<ApiResponse<List<string>>> GetAllPoNumber()
        {
            try
            {
                var PONumber = _context.CooPos
                    .Select(x => x.PoNum)
                    .Distinct()
                    .ToList();

                return new ApiResponse<List<string>>("PO Numbers fetched successfully", PONumber, null, PONumber.Count);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<string>>("Error fetching PO Numbers", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<GetAllInvoiceDto>>> GetAllPoNumber(GetAllDetailsInvoice getAllDetailsInvoice)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(getAllDetailsInvoice?.PoNumber))
                {
                    return new ApiResponse<List<GetAllInvoiceDto>>("PO Number is required", null, new List<string> { "PoNumber is null or empty." });
                }

                var result = _context.CooPos
                    .Include(x => x.Coo)
                    .Where(x => x.PoNum.ToLower() == getAllDetailsInvoice.PoNumber.ToLower())
                    .ToList();

                if (result == null || !result.Any())
                {
                    return new ApiResponse<List<GetAllInvoiceDto>>("No COO records found for the given PO Number.", null, new List<string> { "No matching COO records." });
                }

                var groupedCoos = new List<Coos>();

                foreach (var item in result)
                {
                    var coo = item.Coo;
                    if (coo == null)
                        continue;

                    var employeeContracts = _context.EmployeeCoos
                    .Include(ec => ec.Emp)
                    .Where(ec => ec.CooId == coo.CooId && ec.Emp.Active)
                    .Where(ec =>
                        (ec.StartCont == null ||
                         (ec.StartCont.Value.Year < getAllDetailsInvoice.Year ||
                          (ec.StartCont.Value.Year == getAllDetailsInvoice.Year && ec.StartCont.Value.Month <= getAllDetailsInvoice.Month))) &&
                        (ec.EndCont == null ||
                         (ec.EndCont.Value.Year > getAllDetailsInvoice.Year ||
                          (ec.EndCont.Value.Year == getAllDetailsInvoice.Year && ec.EndCont.Value.Month >= getAllDetailsInvoice.Month)))
                    )
                    .ToList();


                    var employees = new List<Employess>();

                    foreach (var contract in employeeContracts)
                    {
                        try
                        {
                            var emp = contract.Emp;
                            if (emp == null) continue;

                            var salary = _context.SalaryTrans.FirstOrDefault(x =>
                                x.SlaryMonth == getAllDetailsInvoice.Month &&
                                x.SlaryYear == getAllDetailsInvoice.Year &&
                                x.RefNo == emp.RefNo);

                            employees.Add(new Employess
                            {
                                EmployeeId=emp.RefNo,                              
                                ContractId= contract.Id,                              
                                SalaryId= salary?.TransId,                              
                                EmpName = emp.EmpName,
                                ArabicName = emp.ArabicName,
                                Transportion = salary?.Transportation ?? 0,
                                Mobile = salary?.Mobile ?? 0,
                                Laptop = salary?.Laptop ?? 0,
                                PayableSalaryUsd = salary?.SalaryUsd ?? 0,
                                PayableSalarySYP = salary?.Ammount ?? 0,
                                SalaryUsd = salary?.SalaryUsd ?? 0,
                                LaptopRent=salary?.LaptopRent ?? 0,
                            });
                        }
                        catch (Exception innerEx)
                        {
                            // Log the error if needed
                            continue; // Don't stop the execution for other employees
                        }
                    }

                    groupedCoos.Add(new Coos
                    {
                        CooNumber = coo.CooNumber,
                        CooDate = coo.CooDate,
                        Employess = employees.Where(e => e != null).ToList() // Ensure no null employees here
                    });
                }

                var responseDto = new GetAllInvoiceDto
                {
                    Coos = groupedCoos
                };

                return new ApiResponse<List<GetAllInvoiceDto>>("Employee Salary fetched successfully", new List<GetAllInvoiceDto> { responseDto }, null, 1);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetAllInvoiceDto>>("Error fetching Employee Salary", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<IEnumerable<InvoiceDto>>> GetAllInvoicesAsync(FilterModel filterRequest)
        {
            if (filterRequest == null)
            {
                return new ApiResponse<IEnumerable<InvoiceDto>>(
                    "Error: Filter model cannot be null",
                    null,
                    new List<string> { "Filter model is null" },
                    0
                );
            }

            try
            {
                var query = _context.Invoice.AsQueryable();

                int totalCount = await query.CountAsync();

                // Apply Global Filter
                if (!string.IsNullOrEmpty(filterRequest.GlobalFilter))
                {
                    var searchTerm = filterRequest.GlobalFilter.ToLower();
                    query = query.Where(invoice =>
                        (invoice.ClientName != null && invoice.ClientName.ToLower().Contains(searchTerm)) ||
                        (invoice.CooNo != null && invoice.CooNo.ToLower().Contains(searchTerm)) ||
                        (invoice.Subject != null && invoice.Subject.ToLower().Contains(searchTerm)) ||
                        (invoice.BankName != null && invoice.BankName.ToLower().Contains(searchTerm)) ||
                        (invoice.AccountNo != null && invoice.AccountNo.ToLower().Contains(searchTerm)) ||
                        (invoice.AccountName != null && invoice.AccountName.ToLower().Contains(searchTerm))
                    );
                }

                // Apply filters from filterRequest.Filters
                if (filterRequest.Filters != null && filterRequest.Filters.Any())
                {
                    foreach (var filter in filterRequest.Filters)
                    {
                        string field = filter.Key;
                        string filterJsonString = System.Text.Json.JsonSerializer.Serialize(filter.Value);
                        JsonElement filterValue = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(filterJsonString);

                        if (!filterValue.TryGetProperty("Value", out JsonElement valueElement)) continue;
                        if (valueElement.ValueKind == JsonValueKind.Null || string.IsNullOrEmpty(valueElement.ToString())) continue;

                        if (filterValue.TryGetProperty("MatchMode", out JsonElement matchModeElement))
                        {
                            string matchMode = matchModeElement.GetString()?.ToLower();
                            string value = valueElement.ToString().ToLower();

                            switch (field.ToLower())
                            {
                                case "id":
                                    if (int.TryParse(value, out int idValue))
                                        query = query.Where(i => i.Id == idValue);
                                    break;

                                case "clientname":
                                    query = ApplyTextFilter(query, i => i.ClientName, value, matchMode);
                                    break;
                                case "invoicenumber":
                                    query = ApplyTextFilter(query, i => i.InvoiceNumber, value, matchMode);
                                    break;

                                case "coono":
                                    query = ApplyTextFilter(query, i => i.CooNo, value, matchMode);
                                    break;

                                case "coodate":
                                    if (DateOnly.TryParse(value, out DateOnly cooDate))
                                        query = query.Where(i => i.CooDate == cooDate);
                                    break;

                                case "ponumber":
                                    query = ApplyTextFilter(query, i => i.PoNumber, value, matchMode);
                                    break;

                                case "subject":
                                    query = ApplyTextFilter(query, i => i.Subject, value, matchMode);
                                    break;

                                case "totalwithoutbounes":
                                    if (decimal.TryParse(value, out decimal totalWithoutBounes))
                                        query = query.Where(i => i.TotalWithoutBounes.HasValue && (decimal)i.TotalWithoutBounes.Value == totalWithoutBounes);
                                    break;

                                case "icifees":
                                    if (decimal.TryParse(value, out decimal iciFees))
                                        query = query.Where(i => i.ICIFees.HasValue && (decimal)i.ICIFees.Value == iciFees);
                                    break;

                                case "laptoppaid":
                                    if (decimal.TryParse(value, out decimal laptopPaid))
                                        query = query.Where(i => i.LaptopPaid.HasValue && (decimal)i.LaptopPaid.Value == laptopPaid);
                                    break;

                                case "laptoprent":
                                    if (decimal.TryParse(value, out decimal laptopRent))
                                        query = query.Where(i => i.LaptopRent.HasValue && (decimal)i.LaptopRent.Value == laptopRent);
                                    break;

                                case "transportationn":
                                    if (decimal.TryParse(value, out decimal transportationn))
                                        query = query.Where(i => i.Transportation.HasValue && (decimal)i.Transportation.Value == transportationn);
                                    break;

                                case "mobile":
                                    if (decimal.TryParse(value, out decimal mobile))
                                        query = query.Where(i => i.Mobile.HasValue && (decimal)i.Mobile.Value == mobile);
                                    break;

                                case "grandtotal":
                                    if (decimal.TryParse(value, out decimal grandTotal))
                                        query = query.Where(i => i.GrandTotal.HasValue && (decimal)i.GrandTotal.Value == grandTotal);
                                    break;
                                case "bankname":
                                    query = ApplyTextFilter(query, i => i.BankName, value, matchMode);
                                    break;

                                case "accountno":
                                    query = ApplyTextFilter(query, i => i.AccountNo, value, matchMode);
                                    break;

                                case "accountname":
                                    query = ApplyTextFilter(query, i => i.AccountName, value, matchMode);
                                    break;

                                case "cancel":
                                    if (bool.TryParse(value, out bool cancel))
                                        query = query.Where(i => i.Cancel == cancel);
                                    break;
                            }

                        }
                    }
                }

                // Apply sorting
                if (filterRequest?.MultiSortMeta?.Any() == true)
                {
                    IOrderedQueryable<Invoice> orderedQuery = null;
                    foreach (var sort in filterRequest.MultiSortMeta)
                    {
                        var field = sort.Field;
                        var sortOrder = sort.Order;

                        var property = typeof(Invoice).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (property == null) continue;

                        var parameter = Expression.Parameter(typeof(Invoice), "x");
                        var propertyAccess = Expression.Property(parameter, property);
                        var lambda = Expression.Lambda(propertyAccess, parameter);

                        var methodName = sortOrder == 1 ? "OrderBy" : "OrderByDescending";
                        var resultExpression = Expression.Call(typeof(Queryable), methodName, new[] { typeof(Invoice), property.PropertyType }, query.Expression, Expression.Quote(lambda));

                        query = query.Provider.CreateQuery<Invoice>(resultExpression);
                    }
                }

                // Get result
                var invoices = await query.ToListAsync();

                // Mapping to DTO
                var invoiceDtos = invoices.Select(invoice => new InvoiceDto
                {
                    Id = invoice.Id,
                    ClientName = invoice.ClientName,
                    CooNo = invoice.CooNo,
                    CooDate = invoice.CooDate,
                    PoNumber = invoice.PoNumber,
                    Subject = invoice.Subject,
                    TotalWithoutBounes = invoice.TotalWithoutBounes,
                    ICIFees = invoice.ICIFees,
                    LaptopPaid = invoice.LaptopPaid,
                    LaptopRent = invoice.LaptopRent,
                    Transportation = invoice.Transportation,
                    Mobile = invoice.Mobile,
                    GrandTotal = invoice.GrandTotal,
                    BankNme = invoice.BankName,
                    AccountNo = invoice.AccountNo,
                    AccountName = invoice.AccountName,
                    Cancel = invoice.Cancel,
                    InvoiceNumber = invoice.InvoiceNumber,
                }).ToList();

                // Apply Pagination
                invoiceDtos = invoiceDtos
                    .Skip(filterRequest.First)
                    .Take(filterRequest.Rows > 0 ? filterRequest.Rows : 10)
                    .ToList();

                return new ApiResponse<IEnumerable<InvoiceDto>>("Invoices fetched successfully", invoiceDtos, null, totalCount);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<InvoiceDto>>(
                    "Error occurred while fetching invoices",
                    null,
                    new List<string> { ex.Message },
                    0
                );
            }
        }
        private IQueryable<Invoice> ApplyTextFilter(IQueryable<Invoice> query, Expression<Func<Invoice, string>> propertyExpression, string value, string matchMode)
        {
            switch (matchMode)
            {
                case "equals":
                    return query.Where(Expression.Lambda<Func<Invoice, bool>>(
                        Expression.Equal(propertyExpression.Body, Expression.Constant(value)), propertyExpression.Parameters));
                case "contains":
                    return query.Where(invoice => EF.Functions.Like(propertyExpression.Compile().Invoke(invoice), $"%{value}%"));
                case "startswith":
                    return query.Where(invoice => EF.Functions.Like(propertyExpression.Compile().Invoke(invoice), $"{value}%"));
                case "endswith":
                    return query.Where(invoice => EF.Functions.Like(propertyExpression.Compile().Invoke(invoice), $"%{value}"));
                case "notequals":
                    return query.Where(invoice => propertyExpression.Compile().Invoke(invoice) != value);
                default:
                    return query;
            }
        }


        // 2. Get Invoice by ID
        public async Task<ApiResponse<InvoiceDto>> GetInvoiceByIdAsync(int id)
        {
            try
            {
                var invoice = await _context.Invoice.FindAsync(id);
                if (invoice == null)
                {
                    return new ApiResponse<InvoiceDto>("Invoice not found", null, new List<string> { "Invoice with the provided ID not found" });
                }

                var invoiceDto = new InvoiceDto
                {
                    Id = invoice.Id,
                    ClientName = invoice.ClientName,
                    CooNo = invoice.CooNo,
                    CooDate = invoice.CooDate,
                    PoNumber = invoice.PoNumber,
                    Subject = invoice.Subject,
                    TotalWithoutBounes = invoice.TotalWithoutBounes,
                    ICIFees = invoice.ICIFees,
                    LaptopPaid = invoice.LaptopPaid,
                    LaptopRent = invoice.LaptopRent,
                    Transportation = invoice.Transportation,
                    Mobile = invoice.Mobile,
                    GrandTotal = invoice.GrandTotal,
                    BankNme = invoice.BankName,
                    AccountNo = invoice.AccountNo,
                    AccountName = invoice.AccountName,
                    Cancel=invoice.Cancel,
                    InvoiceNumber = invoice.InvoiceNumber

                };

                return new ApiResponse<InvoiceDto>("Invoice fetched successfully", invoiceDto, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<InvoiceDto>("Error occurred while fetching invoice", null, new List<string> { ex.Message });
            }
        }

        // 3. Create Invoice
        public async Task<ApiResponse<string>> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto)
        {
            try
            {
                if (createInvoiceDto == null)
                {
                    return new ApiResponse<string>("Invalid input", null, new List<string> { "Invoice data is required" });
                }

                var invoice = new Invoice
                {
                    ClientName = createInvoiceDto.ClientName,
                    CooNo = createInvoiceDto.CooNo,
                    CooDate = createInvoiceDto.CooDate,
                    PoNumber = createInvoiceDto.PoNumber,
                    Subject = createInvoiceDto.Subject,
                    TotalWithoutBounes = createInvoiceDto.TotalWithoutBounes,
                    ICIFees = createInvoiceDto.ICIFees,
                    LaptopPaid = createInvoiceDto.LaptopPaid,
                    LaptopRent = createInvoiceDto.LaptopRent,
                    Transportation = createInvoiceDto.Transportation,
                    Mobile = createInvoiceDto.Mobile,
                    GrandTotal = createInvoiceDto.GrandTotal,
                    BankName = createInvoiceDto.BankNme,
                    AccountNo = createInvoiceDto.AccountNo,
                    AccountName = createInvoiceDto.AccountName,
                    InvoiceNumber = createInvoiceDto.InvoiceNumber
                };

                await _context.Invoice.AddAsync(invoice);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    return new ApiResponse<string>("Error creating invoice", null, new List<string> { "Unable to save invoice to database." });
                }

                return new ApiResponse<string>("Invoice created successfully", invoice.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while creating the invoice", null, new List<string> { ex.Message });
            }
        }

        // 4. Update Invoice
        public async Task<ApiResponse<string>> UpdateInvoiceAsync(UpdateInvoiceDto updateInvoiceDto)
        {
            try
            {
                var invoice = await _context.Invoice.FindAsync(updateInvoiceDto.Id);
                if (invoice == null)
                {
                    return new ApiResponse<string>("Invoice not found", null, new List<string> { "Invoice with the provided ID not found" });
                }

                invoice.Cancel = true;

                _context.Invoice.Update(invoice);

                var Addinvoice = new Invoice
                {
                    ClientName = updateInvoiceDto.ClientName,
                   CooNo = updateInvoiceDto.CooNo,
                   CooDate = updateInvoiceDto.CooDate,
                   PoNumber = updateInvoiceDto.PoNumber,
                   Subject = updateInvoiceDto.Subject,
                   TotalWithoutBounes = updateInvoiceDto.TotalWithoutBounes,
                   ICIFees = updateInvoiceDto.ICIFees,
                   LaptopPaid = updateInvoiceDto.LaptopPaid,
                   LaptopRent = updateInvoiceDto.LaptopRent,
                   Transportation = updateInvoiceDto.Transportation,
                   Mobile = updateInvoiceDto.Mobile,
                   GrandTotal = updateInvoiceDto.GrandTotal,
                   BankName = updateInvoiceDto.BankNme,
                   AccountNo = updateInvoiceDto.AccountNo,
                   AccountName = updateInvoiceDto.AccountName,
                   Cancel = false,
                   InvoiceNumber= updateInvoiceDto.InvoiceNumber,
                };

                await _context.Invoice.AddAsync(Addinvoice);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    return new ApiResponse<string>("Error updating invoice", null, new List<string> { "Unable to update invoice" });
                }

                return new ApiResponse<string>("Invoice updated successfully", invoice.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while updating the invoice", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> CancelInvoiceAsync(int id)
        {
            try
            {
                var invoice = await _context.Invoice.FindAsync(id);
                if (invoice == null)
                {
                    return new ApiResponse<string>("Invoice not found", null, new List<string> { "Invoice with the provided ID not found" });
                }

                invoice.Cancel = true;
                _context.Invoice.Update(invoice);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    return new ApiResponse<string>("Error canceling invoice", null, new List<string> { "Unable to update invoice cancel status" });
                }

                return new ApiResponse<string>("Invoice canceled successfully", invoice.Id.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while canceling the invoice", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<string>> GetLastInvoiceNumber(int ClientId)
        {
            try
            {
                var client = _context.Clients.FirstOrDefault(x => x.ClientId == ClientId);
                if (client == null)
                {
                    // جلب أول فاتورة تطابق اسم العميل (غير حساس لحالة الأحرف)
                    var invoice = _context.Invoice
                     .Where(x => x.ClientName.ToLower() == client.ClientName.ToLower())
                     .OrderByDescending(x => x.InvoiceNumber)  // أو InvoiceDate لو متوفر
                     .FirstOrDefault();

                    if (invoice != null)
                    {
                        return new ApiResponse<string>("Success", invoice.InvoiceNumber, null);
                    }

                    else
                    {
                        if (client != null)
                        {
                            string format = client.Format ?? "";

                            // استبدال جزء السنة {Year}=xxxx بالسنة الحالية
                            int currentYear = DateTime.Now.Year;
                            format = System.Text.RegularExpressions.Regex.Replace(format, @"\{Year\}(=\d+)?", $"{{Year}}={currentYear}");

                            // استخراج قيمة Id لو موجودة
                            var match = System.Text.RegularExpressions.Regex.Match(format, @"\{Id\}=(\d+)");


                            if (match.Success)
                            {
                                string idValue = match.Groups[1].Value;
                                return new ApiResponse<string>("Success", idValue, null);
                            }
                            else
                            {
                                return new ApiResponse<string>("Id not found in client format", null, null);
                            }
                        }
                        else
                        {
                            return new ApiResponse<string>("Client not found", null, null);
                        }
                    }
                }
                else
                {
                    return new ApiResponse<string>("this client is not found", null, new List<string> { "This client is not found"});
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while getting the last invoice number", null, new List<string> { ex.Message });
            }
        }




    }
}
