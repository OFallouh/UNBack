namespace UNNew.DTOS.BankDtos
{
    public class AddAccountCompanyDto
    {
        public int BanksId { get; set; }
        public string? Region { get; set; }
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
    }
}
