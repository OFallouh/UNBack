using AutoMapper;
using UNNew.DTOS;
using UNNew.DTOS.ClientDtos;
using UNNew.Models;
using UNNew.Response;
using Microsoft.EntityFrameworkCore;
using UNNew.Repository.Interfaces;
using Microsoft.Data.SqlClient;


namespace UNNew.Repository.Services
{
    public class ClientManagmentService : IClientManagmentService
    {
        private readonly UNDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ClientManagmentService(UNDbContext uNDbContext, IMapper mapper, IConfiguration configuration)
        {
            _context = uNDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        // Get All Clients
        public async Task<ApiResponse<IEnumerable<ClientDto>>> GetAllClientsAsync()
        {
            try
            {
                var clients = await _context.Clients.ToListAsync();
                if (clients == null || !clients.Any())
                {
                    return new ApiResponse<IEnumerable<ClientDto>>("No clients found", null, new List<string> { "No data available" });
                }

                var clientDtos = clients.Select(client => new ClientDto
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName
                }).ToList();

                return new ApiResponse<IEnumerable<ClientDto>>("Clients fetched successfully", clientDtos, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ClientDto>>("Error occurred while fetching clients", null, new List<string> { ex.Message });
            }
        }

        // Get Client By Id
        public async Task<ApiResponse<ClientDto>> GetClientByIdAsync(int clientId)
        {
            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
                if (client == null)
                {
                    return new ApiResponse<ClientDto>("Client not found", null, new List<string> { "Client with the provided ID not found" });
                }

                var lastInvoiceId = "";
                if (client.Format != null)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(client.Format, @"\{Id\}=(\d+)");
                    if (match.Success)
                    {
                        lastInvoiceId = match.Groups[1].Value;
                    }
                }

                var clientDto = new ClientDto
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName,
                    Format = System.Text.RegularExpressions.Regex.Replace(client.Format ?? "", @"(\{\w+\})=\d+", "$1"),
                    LastInvoiceId = lastInvoiceId,
                };


                return new ApiResponse<ClientDto>("Client fetched successfully", clientDto, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClientDto>("Error occurred while fetching client", null, new List<string> { ex.Message });
            }
        }

        // Create Client
        public async Task<ApiResponse<string>> CreateClientAsync(CreateClientDto createClientDto)
        {
            try
            {
                var _connectionString = _configuration.GetConnectionString("DefaultConnection");

                // التحقق من وجود اسم العميل مسبقاً
                string checkQuery = "SELECT COUNT(1) FROM Clients WHERE client_Name = @ClientName";
                bool clientExists;

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // التحقق من وجود الاسم
                    using (var command = new SqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ClientName", createClientDto.ClientName);
                        clientExists = (int)await command.ExecuteScalarAsync() > 0;
                    }

                    if (clientExists)
                    {
                        return new ApiResponse<string>("Client name already exists", null, new List<string> { "A client with the same name already exists." });
                    }

                    // الحصول على آخر client_ID + 1
                    string getMaxClientIdQuery = "SELECT MAX(client_ID) FROM Clients";
                    int newClientId;

                    using (var command = new SqlCommand(getMaxClientIdQuery, connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        newClientId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1;
                    }

                    // السنة الحالية
                    int currentYear = DateTime.Now.Year;

                    // الفورمات المأخوذة من المستخدم أو قيمة افتراضية
                    string format = createClientDto.Format;

                    // استبدال {Year} بالقيمة الحالية (سواء كانت موجودة مسبقاً مع أو بدون رقم)
                    format = System.Text.RegularExpressions.Regex.Replace(format, @"\{Year\}(=\d+)?", $"{{Year}}={currentYear}");

                    // استبدال {Id} بالقيمة المرسلة من lastInvoiceId إذا موجودة، أو تترك كما هي إذا لا
                    if (createClientDto.lastInvoiceId.HasValue)
                    {
                        // إذا في id مع رقم مسبقاً استبدله
                        if (System.Text.RegularExpressions.Regex.IsMatch(format, @"\{Id\}(=\d+)?"))
                        {
                            format = System.Text.RegularExpressions.Regex.Replace(format, @"\{Id\}(=\d+)?", $"{{Id}}={createClientDto.lastInvoiceId.Value}");
                        }
                        else
                        {
                            // لو ما كان موجود id بالصيغة، ممكن تضيفه في بداية الفورمات مثلاً
                            format = $"{{Id}}={createClientDto.lastInvoiceId.Value}-" + format;
                        }
                    }

                    // تنفيذ الإدخال
                    string insertClientQuery = "INSERT INTO Clients (client_ID, client_Name, Format) VALUES (@ClientId, @ClientName, @Format)";

                    using (var command = new SqlCommand(insertClientQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ClientId", newClientId);
                        command.Parameters.AddWithValue("@ClientName", createClientDto.ClientName);
                        command.Parameters.AddWithValue("@Format", format);

                        await command.ExecuteNonQueryAsync();
                    }

                    return new ApiResponse<string>("Client created successfully", newClientId.ToString(), null);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error while creating client", null, new List<string> { ex.Message });
            }
        }



        // Update Client
        public async Task<ApiResponse<string>> UpdateClientAsync(UpdateClientDto updateClientDto)
        {
            try
            {
                var client = await _context.Clients.FindAsync(updateClientDto.ClientId);
                if (client == null)
                {
                    return new ApiResponse<string>("Client not found", null, new List<string> { "Client with the provided ID not found" });
                }

                // التحقق من وجود اسم العميل مكرر
                bool clientExists = await _context.Clients
                    .AnyAsync(c => c.ClientName == updateClientDto.ClientName && c.ClientId != updateClientDto.ClientId);
                if (clientExists)
                {
                    return new ApiResponse<string>("Client name already exists", null, new List<string> { "A client with the same name already exists." });
                }

                // تحديث الاسم
                client.ClientName = updateClientDto.ClientName;

                // السنة الحالية
                int currentYear = DateTime.Now.Year;

                // الحصول على الفورمات المدخل من المستخدم (لو متوفر) أو fallback للفورمات القديمة
                string format = updateClientDto.Format ?? client.Format;

                // تحديث قيمة {Year}
                format = System.Text.RegularExpressions.Regex.Replace(format, @"\{Year\}(=\d+)?", $"{{Year}}={currentYear}");

                // تحديث قيمة {Id} إذا كانت موجودة وإلا تضيفها لو lastInvoiceId متوفر
                if (updateClientDto.lastInvoiceId.HasValue)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(format, @"\{Id\}(=\d+)?"))
                    {
                        format = System.Text.RegularExpressions.Regex.Replace(format, @"\{Id\}(=\d+)?", $"{{Id}}={updateClientDto.lastInvoiceId.Value}");
                    }
                    else
                    {
                        format = $"{{Id}}={updateClientDto.lastInvoiceId.Value}-" + format;
                    }
                }

                client.Format = format;

                _context.Clients.Update(client);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>("Client updated successfully", client.ClientId.ToString(), null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>("Error occurred while updating the client", null, new List<string> { ex.Message });
            }
        }



        // Delete Client
        public async Task<ApiResponse<string>> DeleteClientAsync(int id)
        {
            try
            {
                var coopo = _context.EmployeeCoos
                    .FirstOrDefault(x => x.Coo.ClientId == id);

                if (coopo != null)
                {
                    return new ApiResponse<string>(
                        "Cannot delete this client because it is associated with an active contract.",
                        null,
                        new List<string> { "Client is associated with an active contract." }
                    );
                }

                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    return new ApiResponse<string>(
                        "Client not found.",
                        null,
                        new List<string> { "Client with the provided ID was not found." }
                    );
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>(
                    "Client deleted successfully.",
                    null,
                    new List<string>() // نرجع List فاضية بدل null لضمان Success = true فقط عند عدم وجود Errors
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    "An error occurred while deleting the client.",
                    null,
                    new List<string> { ex.Message }
                );
            }
        }


    }
}

