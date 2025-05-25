namespace UNNew.DTOS.ClientDtos
{
    public class UpdateClientDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string? Format { get; set; }
        public int? lastInvoiceId { get; set; }
    }
}
