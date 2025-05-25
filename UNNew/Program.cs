using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UNNew.Helpers;
using UNNew.Mapping;
using UNNew.Models;
using UNNew.Repository.Interfaces;
using UNNew.Repository.Services;
using UNNew.BackGroundServices;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Add services to the container

// 1. Adding Controllers for API Endpoints
builder.Services.AddControllers();

// 2. Adding Swagger/OpenAPI Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Configuring the Database Context (SQL Server)
builder.Services.AddDbContext<UNDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    )
);

// 4. Adding AutoMapper for object mapping
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 5. Registering Application Services (Scoped)
builder.Services.AddScoped<LogFilterAttribute>();
builder.Services.AddScoped<IUserManagmentService, UserManagmentService>();
builder.Services.AddScoped<IRoleManagmentService, RoleManagmentService>();
builder.Services.AddScoped<IBankManagmentService, BankManagmentService>();
builder.Services.AddScoped<ICityManagmentService, CityManagmentService>();
builder.Services.AddScoped<IClientManagmentService, ClientManagmentService>();
builder.Services.AddScoped<ICooManagmentService, CooManagmentService>();
builder.Services.AddScoped<ICurrencyManagmentService, CurrencyManagmentService>();
builder.Services.AddScoped<IPurchaseOrderManagmentService, PurchaseOrderManagmentService>();
builder.Services.AddScoped<ITeamManagmentService, TeamManagmentService>();
builder.Services.AddScoped<ITypeOfContractManagmentService, TypeOfContractManagmentService>();
builder.Services.AddScoped<IInsuranceManagmentService, InsuranceManagmentService>();
builder.Services.AddScoped<ILaptopRentManagmentService, LaptopRentManagmentService>();
builder.Services.AddScoped<ISalaryManagmentService, SalaryManagmentService>();
builder.Services.AddScoped<IUNRateManagmentService, UNRateManagmentService>();
builder.Services.AddScoped<IUNEmployeeManagmentService, UNEmployeeManagmentService>();
builder.Services.AddScoped<IContractManagmentService, ContractManagmentService>();
builder.Services.AddScoped<IInvoiceManagmentService, InvoiceManagmentSerice>();
builder.Services.AddScoped<IAccountCompanyManagmentService, AccountCompanyManagmentService>();
builder.Services.AddScoped<IUnLaptopCompensationService, UnLaptopCompensationService>();
builder.Services.AddScoped<ITransportCompensationService, TransportCompensationService>();
builder.Services.AddScoped<IUnMonthLeaveService, UnMonthLeaveService>();

builder.Services.AddHostedService<BackGroundService>();

// 🔹 Adding CORS for cross-origin requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// 6. Adding HttpContextAccessor for accessing HTTP context
builder.Services.AddHttpContextAccessor();

// 🔹 Adding JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// 🔹 Build the application
var app = builder.Build();

// 🔹 Configure the HTTP request pipeline

// 1. Enabling Swagger for API Documentation
app.UseSwagger();
app.UseSwaggerUI();

// 2. Redirect HTTP to HTTPS (Optional)
app.UseHttpsRedirection();

// 3. Enable CORS
app.UseCors("AllowAll");

// 4. Enabling Authentication & Authorization Middleware
app.UseAuthentication(); // 🟢 إضافة Middleware المصادقة
app.UseAuthorization();

// ⚠️ ضع هذا قبل app.MapControllers();
var logoBankPath = builder.Configuration["FileStorage:LogoBankUrlPath"];


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(logoBankPath),
    RequestPath = "/LogoBank",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});



// 5. Mapping the Controllers to Endpoints
app.MapControllers();

// 🔹 Run the application
app.Run();
