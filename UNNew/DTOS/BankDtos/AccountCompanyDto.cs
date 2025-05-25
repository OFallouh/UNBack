namespace UNNew.DTOS.BankDtos
{
    public class AccountCompanyDto
    {
        public int AccountCompanyId { get; set; }
        public int BanksId { get; set; }
        public string? BankName { get; set; } // اختياري: إذا بدك تعرض اسم البنك بدل Id فقط
        public string? Region { get; set; }
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankLogoUrl { get; set; }
    }
}
