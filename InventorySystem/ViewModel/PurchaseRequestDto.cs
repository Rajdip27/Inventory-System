namespace InventorySystem.ViewModel
{
    public class PurchaseRequestDto
    {
        public long Id { get; set; } 
        public long UserId { get; set; }
        public long SupplierId { get; set; }
        public string Note { get; set; }
        public List<PurchaseItemDto> Items { get; set; } = new();
    }
    public class PurchaseItemDto
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
