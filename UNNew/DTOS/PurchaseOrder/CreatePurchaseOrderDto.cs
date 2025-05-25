namespace UNNew.DTOS.PurchaseOrder
{
    public class CreatePurchaseOrderDto
    {
        public string PoNo { get; set; } = null!;
        public double? PoAmount { get; set; }
        public int Cooid { get; set; }
    }
}
