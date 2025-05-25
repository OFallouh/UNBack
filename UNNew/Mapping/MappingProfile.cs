using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;
using UNNew.DTOS;
using UNNew.Models;
using UNNew.DTOS.UserDTO;
using UNNew.DTOS.UNEmployeeDtos;
using UNNew.DTOS.ContractDtos;
namespace UNNew.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, LoginUserDto>().ReverseMap();



            //----------------//CreateUNEmployeeDto //------------------------//
            CreateMap<CreateUNEmployeeDto, UnEmp>()
                 .ForMember(dest => dest.EmpName, opt => opt.MapFrom(src => src.personal.EmpName))
                 .ForMember(dest => dest.ArabicName, opt => opt.MapFrom(src => src.personal.ArabicName))
                 .ForMember(dest => dest.FatherNameArabic, opt => opt.MapFrom(src => src.personal.FatherNameArabic))
                 .ForMember(dest => dest.MotherNameArabic, opt => opt.MapFrom(src => src.personal.MotherNameArabic))
                 .ForMember(dest => dest.IdNo, opt => opt.MapFrom(src => src.personal.IdNo))
                 .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.personal.EmailAddress))
                 .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.personal.MobileNo))
                 .ForMember(dest => dest.IsDelegated, opt => opt.MapFrom(src => src.bankInfo.IsDelegated))
                 .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.personal.Gender.ToString())) // تحويل enum إلى string
                 .ForMember(dest => dest.OldEmployment, opt => opt.MapFrom(src => src.personal.OldEmployment.ToString())) // تحويل enum إلى string
                 .ForMember(dest => dest.SecurityCheck, opt => opt.MapFrom(src => src.personal.SecurityCheck.ToString())) // تحويل enum إلى string

         

                 .ForMember(dest => dest.BankId, opt => opt.MapFrom(src => src.bankInfo.BankId))
                 .ForMember(dest => dest.TypeOfAcc, opt => opt.MapFrom(src => src.bankInfo.TypeOfAcc))
                 .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.bankInfo.AccountNumber));// بدون ToString()

            //.ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.contractInfo.ClientId))
            //.ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.contractInfo.TeamId))
            //.ForMember(dest => dest.CooId, opt => opt.MapFrom(src => src.contractInfo.CooId))
            //.ForMember(dest => dest.CooPoId, opt => opt.MapFrom(src => src.contractInfo.CooPoId))
            //.ForMember(dest => dest.ContractSigned, opt => opt.MapFrom(src => src.contractInfo.ContractSigned))
            //.ForMember(dest => dest.ContractStartDate, opt => opt.MapFrom(src => src.contractInfo.ContractStartDate))
            //.ForMember(dest => dest.ContractEndDate, opt => opt.MapFrom(src => src.contractInfo.ContractEndDate))
            //.ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.contractInfo.CityId))
            //.ForMember(dest => dest.Tittle, opt => opt.MapFrom(src => src.contractInfo.Tittle))
            //.ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.contractInfo.Salary))
            //.ForMember(dest => dest.Transportation, opt => opt.MapFrom(src => src.contractInfo.Transportation))
            //.ForMember(dest => dest.Laptop, opt => opt.MapFrom(src => src.contractInfo.Laptop))
            //.ForMember(dest => dest.IsMobile, opt => opt.MapFrom(src => src.contractInfo.IsMobile))
            //.ForMember(dest => dest.TypeOfContractId, opt => opt.MapFrom(src => src.contractInfo.TypeOfContractId))
            //.ForMember(dest => dest.InsuranceLife, opt => opt.MapFrom(src => src.contractInfo.InsuranceLife))
            //.ForMember(dest => dest.InsuranceMedical, opt => opt.MapFrom(src => src.contractInfo.InsuranceMedical)) // إضافة InsuranceMedical
            //.ForMember(dest => dest.StartLifeDate, opt => opt.MapFrom(src => src.contractInfo.StartLifeDate.HasValue
            //   ? DateOnly.FromDateTime(src.contractInfo.StartLifeDate.Value)
            //   : (DateOnly?)null))

            //.ForMember(dest => dest.EndLifeDate, opt => opt.MapFrom(src => src.contractInfo.EndLifeDate.HasValue
            //   ? DateOnly.FromDateTime(src.contractInfo.EndLifeDate.Value)
            //   : (DateOnly?)null))

            //.ForMember(dest => dest.MotherNameArabic, opt => opt.MapFrom(src => src.personal.MotherNameArabic))
            //.ForMember(dest => dest.FatherNameArabic, opt => opt.MapFrom(src => src.personal.FatherNameArabic))

            //.ForMember(dest => dest.SuperVisor, opt => opt.MapFrom(src => src.contractInfo.SuperVisor))
            //.ForMember(dest => dest.AreaManager, opt => opt.MapFrom(src => src.contractInfo.AreaManager))
            //.ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.contractInfo.ProjectName));

            //----------------//UpdateUNEmployeeDto //------------------------//
            CreateMap<UpdateUNEmployeeDto, UnEmp>()

               .ForMember(dest => dest.EmpName, opt => opt.MapFrom(src => src.personal.EmpName))
               .ForMember(dest => dest.ArabicName, opt => opt.MapFrom(src => src.personal.ArabicName))
               .ForMember(dest => dest.FatherNameArabic, opt => opt.MapFrom(src => src.personal.FatherNameArabic))
               .ForMember(dest => dest.MotherNameArabic, opt => opt.MapFrom(src => src.personal.MotherNameArabic))
               .ForMember(dest => dest.IdNo, opt => opt.MapFrom(src => src.personal.IdNo))
               .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.personal.EmailAddress))
               .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.personal.MobileNo))
               .ForMember(dest => dest.IsDelegated, opt => opt.MapFrom(src => src.bankInfo.IsDelegated))
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.personal.Gender.ToString())) // تحويل enum إلى string
               .ForMember(dest => dest.OldEmployment, opt => opt.MapFrom(src => src.personal.OldEmployment.ToString())) // تحويل enum إلى string
               .ForMember(dest => dest.SecurityCheck, opt => opt.MapFrom(src => src.personal.SecurityCheck.ToString())) // تحويل enum إلى string
      
               .ForMember(dest => dest.BankId, opt => opt.MapFrom(src => src.bankInfo.BankId))
               .ForMember(dest => dest.TypeOfAcc, opt => opt.MapFrom(src => src.bankInfo.TypeOfAcc))
               .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.bankInfo.AccountNumber)); // بدون ToString()

            //.ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.contractInfo.ClientId))
            //.ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.contractInfo.TeamId))
            //.ForMember(dest => dest.CooId, opt => opt.MapFrom(src => src.contractInfo.CooId))
            //.ForMember(dest => dest.CooPoId, opt => opt.MapFrom(src => src.contractInfo.CooPoId))
            //.ForMember(dest => dest.ContractSigned, opt => opt.MapFrom(src => src.contractInfo.ContractSigned))
            //.ForMember(dest => dest.ContractStartDate, opt => opt.MapFrom(src => src.contractInfo.ContractStartDate))
            //.ForMember(dest => dest.ContractEndDate, opt => opt.MapFrom(src => src.contractInfo.ContractEndDate))
            //.ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.contractInfo.CityId))
            //.ForMember(dest => dest.Tittle, opt => opt.MapFrom(src => src.contractInfo.Tittle))
            //.ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.contractInfo.Salary))
            //.ForMember(dest => dest.Transportation, opt => opt.MapFrom(src => src.contractInfo.Transportation))
            //.ForMember(dest => dest.Laptop, opt => opt.MapFrom(src => src.contractInfo.Laptop))
            //.ForMember(dest => dest.IsMobile, opt => opt.MapFrom(src => src.contractInfo.IsMobile))
            //.ForMember(dest => dest.TypeOfContractId, opt => opt.MapFrom(src => src.contractInfo.TypeOfContractId))
            //.ForMember(dest => dest.InsuranceLife, opt => opt.MapFrom(src => src.contractInfo.InsuranceLife))
            //.ForMember(dest => dest.InsuranceMedical, opt => opt.MapFrom(src => src.contractInfo.InsuranceMedical)) // إضافة InsuranceMedical
            // .ForMember(dest => dest.StartLifeDate, opt => opt.MapFrom(src => src.contractInfo.StartLifeDate.HasValue
            //     ? DateOnly.FromDateTime(src.contractInfo.StartLifeDate.Value)
            //     : (DateOnly?)null))

            //  .ForMember(dest => dest.EndLifeDate, opt => opt.MapFrom(src => src.contractInfo.EndLifeDate.HasValue
            //     ? DateOnly.FromDateTime(src.contractInfo.EndLifeDate.Value)
            //     : (DateOnly?)null))
            //.ForMember(dest => dest.MotherNameArabic, opt => opt.MapFrom(src => src.personal.MotherNameArabic))
            //.ForMember(dest => dest.FatherNameArabic, opt => opt.MapFrom(src => src.personal.FatherNameArabic))

            //.ForMember(dest => dest.SuperVisor, opt => opt.MapFrom(src => src.contractInfo.SuperVisor))
            //.ForMember(dest => dest.AreaManager, opt => opt.MapFrom(src => src.contractInfo.AreaManager))
            //.ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.contractInfo.ProjectName));

            //---------------------- //-------------------
            CreateMap<AddContractDto, EmployeeCoo>()
           .ForMember(dest => dest.EmpId, opt => opt.MapFrom(src => src.EmployeeId))
           .ForMember(dest => dest.StartCont, opt => opt.MapFrom(src =>
               src.ContractStartDate.HasValue ? DateOnly.FromDateTime(src.ContractStartDate.Value) : (DateOnly?)null))
           .ForMember(dest => dest.EndCont, opt => opt.MapFrom(src =>
               src.ContractEndDate.HasValue ? DateOnly.FromDateTime(src.ContractEndDate.Value) : (DateOnly?)null))
            .ForMember(dest => dest.Laptop, opt => opt.MapFrom(src => src.LaptopTypeId));

            CreateMap<UpdateContractDto, EmployeeCoo>()

          .ForMember(dest => dest.StartCont, opt => opt.MapFrom(src =>
              src.ContractStartDate.HasValue ? DateOnly.FromDateTime(src.ContractStartDate.Value) : (DateOnly?)null))
          .ForMember(dest => dest.EndCont, opt => opt.MapFrom(src =>
              src.ContractEndDate.HasValue ? DateOnly.FromDateTime(src.ContractEndDate.Value) : (DateOnly?)null))
           .ForMember(dest => dest.Laptop, opt => opt.MapFrom(src => src.LaptopTypeId));

            //.ForMember(dest => dest.EndLifeDate, opt => opt.MapFrom(src => src.contractInfo.EndLifeDate.HasValue
            //   ? DateOnly.FromDateTime(src.contractInfo.EndLifeDate.Value)
            //   : (DateOnly?)null))
        }
    }
}
