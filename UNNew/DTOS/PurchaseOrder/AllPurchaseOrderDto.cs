namespace UNNew.DTOS.PurchaseOrder
{
    public class AllPurchaseOrderDto
    {
        public int OrderId { get; set; }
        public string PoNo { get; set; } = null!;
        public double? PoAmount { get; set; }
        public int Cooid { get; set; }
    }
}
