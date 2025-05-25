using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UNNew.Models;

public partial class UNDbContext : DbContext
{
    public UNDbContext()
    {
    }

    public UNDbContext(DbContextOptions<UNDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Coo> Coos { get; set; }

    public virtual DbSet<CooPo> CooPos { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<EmployeeCoo> EmployeeCoos { get; set; }

    public virtual DbSet<LaptopRent> LaptopRents { get; set; }

    public virtual DbSet<LaptopType> LaptopTypes { get; set; }

    public virtual DbSet<LifeInsurance> LifeInsurances { get; set; }

    public virtual DbSet<LogAction> LogActions { get; set; }

    public virtual DbSet<MedicalInsurance> MedicalInsurances { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SalaryEmployeeCoo> SalaryEmployeeCoos { get; set; }

    public virtual DbSet<SalaryTran> SalaryTrans { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TypeOfAccount> TypeOfAccounts { get; set; }

    public virtual DbSet<TypeOfContract> TypeOfContracts { get; set; }

    public virtual DbSet<UnEmp> UnEmps { get; set; }

    public virtual DbSet<UnRate> UnRates { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<VUnReport> VUnReports { get; set; }
    public virtual DbSet<AccountCompany> AccountCompany { get; set; }
    public virtual DbSet<UnTransportCompensation> UnTransportCompensation { get; set; }
    public virtual DbSet<UnMobileCompensation> UnMobileCompensation { get; set; }
    public virtual DbSet<UnMonthLeave> UnMonthLeave { get; set; }
    public virtual DbSet<Invoice> Invoice { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasKey(e => e.BanksId).HasName("PK__Banks__93FD3405BD7685B1");

            entity.Property(e => e.BanksId)
                .ValueGeneratedNever()
                .HasColumnName("Banks_ID");
            entity.Property(e => e.BanksName)
                .HasMaxLength(100)
                .HasColumnName("Banks_Name");
        });
        modelBuilder.Entity<AccountCompany>(entity =>
        {
            entity.HasOne(e => e.Bank)
              .WithMany(b => b.AccountCompanys)
              .HasForeignKey(e => e.BanksId)
              .OnDelete(DeleteBehavior.Restrict);


        });
        modelBuilder.Entity<UnTransportCompensation>(entity =>
        {
            entity.HasOne(e => e.client)
              .WithMany(b => b.UnTransportCompensations)
              .HasForeignKey(e => e.ClientId)
              .OnDelete(DeleteBehavior.Restrict);


        });
        modelBuilder.Entity<UnMobileCompensation>(entity =>
        {
            entity.HasOne(e => e.client)
              .WithMany(b => b.UnMobileCompensations)
              .HasForeignKey(e => e.ClientId)
              .OnDelete(DeleteBehavior.Restrict);


        });

        modelBuilder.Entity<UnMonthLeave>(entity =>
        {
            entity.HasOne(e => e.Client)
              .WithMany(b => b.UnMonthLeaves)
              .HasForeignKey(e => e.ClientId)
              .OnDelete(DeleteBehavior.Restrict);


        });

        modelBuilder.Entity<LaptopRent>(entity =>
        {
            entity.HasOne(e => e.client)
              .WithMany(b => b.LaptopRents)
              .HasForeignKey(e => e.ClientId)
              .OnDelete(DeleteBehavior.Restrict);


        });
        modelBuilder.Entity<SalaryTran>(entity =>
        {
            entity.HasOne(e => e.EmployeeCoo)
              .WithMany(b => b.SalaryTrans)
              .HasForeignKey(e => e.ContractId)
              .OnDelete(DeleteBehavior.Restrict);


        });

 
        modelBuilder.Entity<UN_Employee_Login>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UN_Employee_Login");

            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Password");

            entity.Property(e => e.EmployeeId)
                .IsRequired()
                .HasColumnName("EmployeeId");

            // تحديد العلاقة مع جدول UN_Emp
            entity.HasOne(d => d.Employee) // جدول UN_Employee_Login يحتوي على العلاقة مع UN_Emp
                .WithMany(p => p.EmployeeLogins) // من الممكن أن يكون لكل UN_Emp العديد من UN_Employee_Login
                .HasForeignKey(d => d.EmployeeId) // المفتاح الخارجي في UN_Employee_Login
                .OnDelete(DeleteBehavior.Cascade); // عند حذف موظف سيتم حذف السجلات المرتبطة به في UN_Employee_Login
        });
        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");

            entity.Property(e => e.CityId).HasColumnName("City_Id");
            entity.Property(e => e.NameAr)
                .HasMaxLength(200)
                .HasColumnName("Name_Ar");
            entity.Property(e => e.NameEn)
                .HasMaxLength(200)
                .HasColumnName("Name_En");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(e => e.ClientId)
                .ValueGeneratedNever()
                .HasColumnName("client_ID");
            entity.Property(e => e.ClientName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("client_Name");
        });

        modelBuilder.Entity<Coo>(entity =>
        {
            entity.ToTable("COO");

            entity.Property(e => e.CooId).HasColumnName("COO_ID");
            entity.Property(e => e.ClientId).HasColumnName("client_ID");
            entity.Property(e => e.CooDate)
                .HasColumnType("datetime")
                .HasColumnName("COO_Date");
            entity.Property(e => e.CooNumber).HasColumnName("COO_Number");
            entity.Property(e => e.CurrencyId).HasColumnName("currency_Id");
            entity.Property(e => e.TotalValue).HasColumnName("Total_Value");

            entity.HasOne(d => d.Client).WithMany(p => p.Coos)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__COO__client_ID__56B3DD81");

            entity.HasOne(d => d.Currency).WithMany(p => p.Coos)
                .HasForeignKey(d => d.CurrencyId)
                .HasConstraintName("FK__COO__currency_Id__6477ECF3");
        });

        modelBuilder.Entity<CooPo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__COO_PO__3214EC07B20430AD");

            entity.ToTable("COO_PO");

            entity.Property(e => e.CooId).HasColumnName("COO_Id");
            entity.Property(e => e.PoNum)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PO_Num");

            entity.HasOne(d => d.Coo).WithMany(p => p.CooPos)
                .HasForeignKey(d => d.CooId)
                .HasConstraintName("FK_COO_COO_PO");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__currency__3214EC275CCABD01");

            entity.ToTable("currency");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EmployeeCoo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC27E13683D0");

            entity.ToTable("Employee_Coo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("City_Id");
            entity.Property(e => e.ClientId).HasColumnName("client_Id");
            entity.Property(e => e.ContractSigned)
                .HasDefaultValue(false)
                .HasColumnName("Contract_signed");
            entity.Property(e => e.CooId).HasColumnName("COO_ID");
            entity.Property(e => e.CooPoId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("COO_PO_Id");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EmpId).HasColumnName("Emp_ID");
            entity.Property(e => e.EndCont).HasColumnName("end_cont");
            entity.Property(e => e.EndLifeDate).HasColumnName("end_life_date");
            entity.Property(e => e.InsuranceLife)
                .HasDefaultValue(false)
                .HasColumnName("Insurance_Life");
            entity.Property(e => e.InsuranceMedical)
                .HasDefaultValue(false)
                .HasColumnName("Insurance_Medical");
            entity.Property(e => e.IsSendCard).HasDefaultValue(false);
            entity.Property(e => e.IsSendContra).HasDefaultValue(false);
            entity.Property(e => e.IsSendMedical)
                .HasDefaultValue(false)
                .HasColumnName("is_send_medical");
            entity.Property(e => e.Laptop).HasColumnName("laptop");
            entity.Property(e => e.ProjectName).HasMaxLength(1000);
            entity.Property(e => e.Salary).HasColumnName("salary");
            entity.Property(e => e.SendInsuranceDate).HasColumnName("send_insurance_date");
            entity.Property(e => e.StartCont).HasColumnName("start_cont");
            entity.Property(e => e.StartLifeDate).HasColumnName("start_life_date");
            entity.Property(e => e.TeamId).HasColumnName("Team_Id");
            entity.Property(e => e.Tittle).HasMaxLength(255);
            entity.Property(e => e.TypeOfContractId).HasColumnName("Type_of_contract_Id");

            entity.HasOne(d => d.City).WithMany(p => p.EmployeeCoos)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK__Employee___City___66603565");

            entity.HasOne(d => d.Client).WithMany(p => p.EmployeeCoos)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Employee___clien__6754599E");

            entity.HasOne(d => d.Coo).WithMany(p => p.EmployeeCoos)
                .HasForeignKey(d => d.CooId)
                .HasConstraintName("FK__Employee___COO_I__0C50D423");

            entity.HasOne(d => d.Emp).WithMany(p => p.EmployeeCoos)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK_Employee_Coo_UN_Emp");

            entity.HasOne(d => d.LaptopNavigation).WithMany(p => p.EmployeeCoos)
                .HasForeignKey(d => d.Laptop)
                .HasConstraintName("FK_LAPTOPTYPE_Employee_Coo");

            entity.HasOne(d => d.Order).WithMany(p => p.EmployeeCoos)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Employee___Order__693CA210");

            entity.HasOne(d => d.Team).WithMany(p => p.EmployeeCoos)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK__Employee___Team___6A30C649");

            entity.HasOne(d => d.TypeOfContract).WithMany(p => p.EmployeeCoos)
                .HasForeignKey(d => d.TypeOfContractId)
                .HasConstraintName("FK__Employee___Type___6B24EA82");
        });

        modelBuilder.Entity<LaptopRent>(entity =>
        {
            entity.ToTable("Laptop_Rent");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LaptopType).HasColumnName("laptop_type");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.LaptopTypeNavigation).WithMany(p => p.LaptopRents)
                .HasForeignKey(d => d.LaptopType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Laptop_Rent_Laptop_Type");
        });

        modelBuilder.Entity<LaptopType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Laptop_T__3214EC07C756C9BA");

            entity.ToTable("Laptop_Type");

            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<LifeInsurance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Life_Ins__3214EC27E59BD333");

            entity.ToTable("Life_Insurance");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CooId).HasColumnName("Coo_Id");
            entity.Property(e => e.EmpId).HasColumnName("Emp_Id");
            entity.Property(e => e.EndDate).HasColumnName("End_Date");
            entity.Property(e => e.StartDate).HasColumnName("Start_Date");

            entity.HasOne(d => d.Coo).WithMany(p => p.LifeInsurances)
                .HasForeignKey(d => d.CooId)
                .HasConstraintName("FK__Life_Insu__Coo_I__6EF57B66");

            entity.HasOne(d => d.Emp).WithMany(p => p.LifeInsurances)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Life_Insu__Emp_I__6FE99F9F");
        });

        modelBuilder.Entity<LogAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LogActio__3214EC07C95DE94D");

            entity.ToTable("LogAction");

            entity.Property(e => e.ActDate).HasColumnType("datetime");
            entity.Property(e => e.ActionName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ControllerName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ObjData).IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MedicalInsurance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medical___3214EC27D604ED29");

            entity.ToTable("Medical_Insurance");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.EmpCooId).HasColumnName("Emp_coo_Id");
            entity.Property(e => e.EndDate).HasColumnName("End_Date");
            entity.Property(e => e.StartDate).HasColumnName("Start_Date");

            entity.HasOne(d => d.EmpCoo).WithMany(p => p.MedicalInsurances)
                .HasForeignKey(d => d.EmpCooId)
                .HasConstraintName("FK__Medical_I__Emp_c__70DDC3D8");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Purchase__C3905BCFEB4A3C76");

            entity.ToTable("Purchase_Orders");

            entity.Property(e => e.Cooid).HasColumnName("COOId");
            entity.Property(e => e.PoAmount).HasColumnName("PO_Amount");
            entity.Property(e => e.PoNo)
                .HasMaxLength(150)
                .HasColumnName("PO_NO");

            entity.HasOne(d => d.Coo).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.Cooid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchase___COOId__71D1E811");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07BF64C847");

            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<SalaryEmployeeCoo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Salary_E__3214EC27CB5A8BBE");

            entity.ToTable("Salary_Employee_Coo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EmployeeCooId).HasColumnName("employee_coo_Id");
            entity.Property(e => e.SalaryId).HasColumnName("Salary_Id");

            entity.HasOne(d => d.EmployeeCoo).WithMany(p => p.SalaryEmployeeCoos)
                .HasForeignKey(d => d.EmployeeCooId)
                .HasConstraintName("FK__Salary_Em__emplo__72C60C4A");

            entity.HasOne(d => d.Salary).WithMany(p => p.SalaryEmployeeCoos)
                .HasForeignKey(d => d.SalaryId)
                .HasConstraintName("FK__Salary_Em__Salar__73BA3083");
        });

        modelBuilder.Entity<SalaryTran>(entity =>
        {
            entity.HasKey(e => e.TransId).HasName("Salary_trans$PrimaryKey");

            entity.ToTable("Salary_trans");

            entity.Property(e => e.TransId).HasColumnName("Trans_ID");
            entity.Property(e => e.AnuallLeave).HasColumnName("Anuall_leave");
            entity.Property(e => e.ClientId).HasColumnName("client_ID");
            entity.Property(e => e.DaysOff).HasColumnName("Days_Off");
            entity.Property(e => e.DaysOn).HasColumnName("Days_On");
            entity.Property(e => e.DownPayment).HasColumnName("Down_Payment");
            entity.Property(e => e.Dsa).HasColumnName("DSA");
            entity.Property(e => e.Laptop)
                .HasDefaultValue(0)
                .HasColumnName("laptop");
            entity.Property(e => e.LaptopRent).HasDefaultValue(0);
            entity.Property(e => e.NetSalary).HasColumnName("Net_Salary");
            entity.Property(e => e.RefNo)
                .HasDefaultValue(0)
                .HasColumnName("Ref_No");
            entity.Property(e => e.SalaryUsd).HasColumnName("Salary_USD");
            entity.Property(e => e.SickLeave).HasColumnName("Sick_Leave");
            entity.Property(e => e.SlaryMonth).HasColumnName("slary_month");
            entity.Property(e => e.SlaryYear).HasColumnName("slary_year");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TeamId).HasColumnName("Team_Id");
            entity.Property(e => e.TransDate)
                .HasPrecision(0)
                .HasColumnName("Trans_Date");
            entity.Property(e => e.UnRate).HasColumnName("UN_rate");

            entity.HasOne(d => d.Client).WithMany(p => p.SalaryTrans)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_client_ID");

            entity.HasOne(d => d.RefNoNavigation).WithMany(p => p.SalaryTrans)
                .HasForeignKey(d => d.RefNo)
                .HasConstraintName("FK_Salary_trans_UN_Emp");

            entity.HasOne(d => d.Team).WithMany(p => p.SalaryTrans)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_Salary_trans_Team");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("Team");

            entity.Property(e => e.TeamId).HasColumnName("Team_ID");
            entity.Property(e => e.ClientId).HasColumnName("client_ID");
            entity.Property(e => e.TeamName).HasMaxLength(200);

            entity.HasOne(d => d.Client).WithMany(p => p.Teams)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__Team__client_ID__778AC167");
        });

        modelBuilder.Entity<TypeOfAccount>(entity =>
        {
            entity.HasKey(e => e.TypeOfAccId);

            entity.ToTable("Type_Of_Account");

            entity.Property(e => e.TypeOfAccId).HasColumnName("Type_of_Acc_Id");
            entity.Property(e => e.NameEn)
                .HasMaxLength(200)
                .HasColumnName("Name_En");
        });

        modelBuilder.Entity<TypeOfContract>(entity =>
        {
            entity.ToTable("Type_of_contract");

            entity.Property(e => e.TypeOfContractId).HasColumnName("Type_of_contract_Id");
            entity.Property(e => e.NmaeEn)
                .HasMaxLength(100)
                .HasColumnName("Nmae_En");
        });

        modelBuilder.Entity<UnEmp>(entity =>
        {
            entity.HasKey(e => e.RefNo).HasName("UN_Emp$PrimaryKey");

            entity.ToTable("UN_Emp");

            entity.Property(e => e.RefNo).HasColumnName("Ref_No");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(50)
                .HasColumnName("Account_Number");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.ArabicName)
                .HasMaxLength(35)
                .HasColumnName("Arabic_Name");
            entity.Property(e => e.AreaManager).HasColumnName("Area Manager");
            entity.Property(e => e.BankId).HasColumnName("Bank_ID");
            entity.Property(e => e.BankName)
                .HasMaxLength(255)
                .HasColumnName("Bank_Name");
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.CityId).HasColumnName("City_Id");
            entity.Property(e => e.ClientId).HasColumnName("Client_ID");
            entity.Property(e => e.ContractCopy)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("Contract_Copy");
            entity.Property(e => e.ContractEndDate)
                .HasPrecision(0)
                .HasColumnName("Contract_end_date");
            entity.Property(e => e.ContractSigned)
                .HasDefaultValue(false)
                .HasColumnName("Contract_signed");
            entity.Property(e => e.ContractStartDate)
                .HasPrecision(0)
                .HasColumnName("Contract_start_date");
            entity.Property(e => e.CooId).HasColumnName("COO_ID");
            entity.Property(e => e.CooNo).HasColumnName("COO No");
            entity.Property(e => e.CooNo1)
                .HasMaxLength(100)
                .HasColumnName("COO_No");
            entity.Property(e => e.CooPoId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("COO_PO_Id");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreationDate).HasPrecision(0);
            entity.Property(e => e.DateOfBirth)
                .HasPrecision(0)
                .HasColumnName("Date_of_Birth");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .HasColumnName("Email_Address");
            entity.Property(e => e.EmpName)
                .HasMaxLength(255)
                .HasColumnName("Emp_Name");
            entity.Property(e => e.EndLifeDate).HasColumnName("end_life_date");
            entity.Property(e => e.FatherNameArabic).HasMaxLength(100);
            entity.Property(e => e.FlagContract).HasColumnName("flag_Contract");
            entity.Property(e => e.Gender)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IdCopy)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("ID_Copy");
            entity.Property(e => e.IdNo).HasColumnName("ID_No");
            entity.Property(e => e.InsuranceDoc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("Insurance_Doc");
            entity.Property(e => e.InsuranceLife)
                .HasDefaultValue(false)
                .HasColumnName("Insurance_Life");
            entity.Property(e => e.InsuranceMedical)
                .HasDefaultValue(false)
                .HasColumnName("Insurance_Medical");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Laptop).HasColumnName("laptop");
            entity.Property(e => e.MedicalCheck)
                .HasDefaultValue(false)
                .HasColumnName("Medical_Check");
            entity.Property(e => e.MobileNo).HasColumnName("Mobile_No");
            entity.Property(e => e.MotherNameArabic).HasMaxLength(100);
            entity.Property(e => e.OldEmployment)
                .HasDefaultValue(false)
                .HasColumnName("Old_employment");
            entity.Property(e => e.ProjectName).HasMaxLength(1000);
            entity.Property(e => e.Salary).HasDefaultValue(0);
            entity.Property(e => e.SecurityCheck)
                .HasDefaultValue(false)
                .HasColumnName("Security_Check");
            entity.Property(e => e.SecurityDoc)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("Security_Doc");
            entity.Property(e => e.SsmaTimeStamp)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnName("SSMA_TimeStamp");
            entity.Property(e => e.StartLifeDate).HasColumnName("start_life_date");
            entity.Property(e => e.Team)
                .HasMaxLength(255)
                .HasComment("List");
            entity.Property(e => e.TeamId).HasColumnName("Team_Id");
            entity.Property(e => e.TimeSheet)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.Tittle).HasMaxLength(255);
            entity.Property(e => e.TotalMonth).HasColumnName("total_month");
            entity.Property(e => e.Transportation).HasDefaultValue(false);
            entity.Property(e => e.TypeOfAcc)
                .HasMaxLength(255)
                .HasColumnName("Type_of_Acc");
            entity.Property(e => e.TypeOfContract)
                .HasMaxLength(255)
                .HasColumnName("Type_of_Contract");
            entity.Property(e => e.TypeOfContractId).HasColumnName("Type_of_contract_Id");
            entity.Property(e => e.UnNo).HasColumnName("UN No");

            entity.HasOne(d => d.Bank).WithMany(p => p.UnEmps)
                .HasForeignKey(d => d.BankId)
                .HasConstraintName("FK__UN_Emp__Bank_ID__0880433F");

            entity.HasOne(d => d.CityNavigation).WithMany(p => p.UnEmps)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_UN_Emp_City");

            entity.HasOne(d => d.Client).WithMany(p => p.UnEmps)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_clientID");

            entity.HasOne(d => d.Coo).WithMany(p => p.UnEmps)
                .HasForeignKey(d => d.CooId)
                .HasConstraintName("FK_UN_Emp_COO");

            entity.HasOne(d => d.LaptopNavigation).WithMany(p => p.UnEmps)
                .HasForeignKey(d => d.Laptop)
                .HasConstraintName("FK_LAPTOPTYPE_UN_EMP");

            entity.HasOne(d => d.Order).WithMany(p => p.UnEmps)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__UN_Emp__OrderId__797309D9");

            entity.HasOne(d => d.TeamNavigation).WithMany(p => p.UnEmps)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_UN_Emp_Team");

            entity.HasOne(d => d.TypeOfContractNavigation).WithMany(p => p.UnEmps)
                .HasForeignKey(d => d.TypeOfContractId)
                .HasConstraintName("FK_UN_Emp_Type_of_contract");
        });

        modelBuilder.Entity<UnRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UnRates__3214EC07BA0898A0");

            entity.Property(e => e.UnRate1).HasColumnName("UnRate");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07B551DFC9");

            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC070A656A78");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId");
        });

        modelBuilder.Entity<VUnReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_Un_Report");

            entity.Property(e => e.AccountNumber)
                .HasMaxLength(50)
                .HasColumnName("Account_Number");
            entity.Property(e => e.ArabicName)
                .HasMaxLength(35)
                .HasColumnName("Arabic_Name");
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.City)
                .HasMaxLength(200)
                .HasColumnName("city");
            entity.Property(e => e.CooNumber).HasColumnName("COO_Number");
            entity.Property(e => e.CooPoId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("COO_PO_Id");
            entity.Property(e => e.Dob)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DOB");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .HasColumnName("Email_Address");
            entity.Property(e => e.EndCont).HasColumnName("end_cont");
            entity.Property(e => e.EndLifeDate).HasColumnName("end_life_date");
            entity.Property(e => e.EnglishName).HasMaxLength(255);
            entity.Property(e => e.FatherNameArabic).HasMaxLength(100);
            entity.Property(e => e.FlagContract).HasColumnName("flag_Contract");
            entity.Property(e => e.Gender)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.HaveFolde)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.IdNo).HasColumnName("ID_No");
            entity.Property(e => e.InsuranceLife).HasColumnName("Insurance_Life");
            entity.Property(e => e.InsuranceMedical).HasColumnName("Insurance_Medical");
            entity.Property(e => e.Laptop).HasMaxLength(200);
            entity.Property(e => e.LastEndContra)
                .HasPrecision(0)
                .HasColumnName("last_end_contra");
            entity.Property(e => e.LastStartContra)
                .HasPrecision(0)
                .HasColumnName("last_start_contra");
            entity.Property(e => e.MobileNo).HasColumnName("Mobile_No");
            entity.Property(e => e.MotherNameArabic).HasMaxLength(100);
            entity.Property(e => e.ProjectName).HasMaxLength(1000);
            entity.Property(e => e.RefNo).HasColumnName("Ref_No");
            entity.Property(e => e.SendInsuranceDate).HasColumnName("send_insurance_date");
            entity.Property(e => e.StartCont).HasColumnName("start_cont");
            entity.Property(e => e.StartLifeDate).HasColumnName("start_life_date");
            entity.Property(e => e.TeamName).HasMaxLength(200);
            entity.Property(e => e.Tittle).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
