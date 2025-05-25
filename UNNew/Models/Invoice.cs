namespace UNNew.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public string? ClientName { get; set; }
        public string? CooNo { get; set; }
        public DateOnly? CooDate { get; set; }  
        public string? PoNumber { get; set; }
        public string? Subject { get; set; }

        public double? TotalWithoutBounes { get; set; }
        public double? ICIFees { get; set; }
        public double? LaptopPaid { get; set; }
        public double? LaptopRent { get; set; }
        public double? Transportation { get; set; }
        public double? Mobile { get; set; }
        public double? GrandTotal { get; set; }

        public string? BankName { get; set; }
        public string? AccountNo { get; set; }
        public string? AccountName { get; set; }
        public bool Cancel { get; set; }
        public string? InvoiceNumber { get; set; }

    }
}
