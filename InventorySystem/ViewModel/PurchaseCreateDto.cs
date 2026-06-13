using InventorySystem.Models.BaseEntities;

namespace InventorySystem.ViewModel;

public class PurchaseCreateDto:BaseEntity
{
    public string InvoiceNo { get; set; }
    public long SupplierId { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.Now;
    public decimal Discount { get; set; }
    public decimal TransportCost { get; set; }
    public string Note { get; set; }
    public List<PurchaseItemDto> Items { get; set; } = new();
}
public class PurchaseItemDto
{
    public long ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }

    public decimal Discount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TransportCost { get; set; }

    public string BatchNo { get; set; }

    public DateTime? ExpiryDate { get; set; }
}
