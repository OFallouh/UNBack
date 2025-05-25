namespace UNNew.DTOS.InvoiceDto
{
    public class UpdateInvoiceDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string CooNo { get; set; }
        public DateOnly CooDate { get; set; }
        public string PoNumber { get; set; }
        public string Subject { get; set; }
        public float TotalWithoutBounes { get; set; }
        public float ICIFees { get; set; }
        public float LaptopPaid { get; set; }
        public float LaptopRent { get; set; }
        public float Transportation { get; set; }
        public float Mobile { get; set; }
        public float GrandTotal { get; set; }
        public string BankNme { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public bool Cancel { get; set; }
        public string? InvoiceNumber { get; set; }
    }
}
