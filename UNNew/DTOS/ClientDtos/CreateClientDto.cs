namespace UNNew.DTOS.ClientDtos
{
    public class CreateClientDto
    {
        public string ClientName { get; set; }
        public string? Format { get; set; }
        public int? lastInvoiceId { get; set; }
    }
}
