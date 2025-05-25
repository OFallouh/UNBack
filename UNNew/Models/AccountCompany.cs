namespace UNNew.Models
{
    public class AccountCompany
    {
        public int AccountCompanyId { get; set; }
        public int BanksId { get; set; }
        public string? Region { get; set; }
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }

        public virtual Bank? Bank { get; set; }
    }
}
