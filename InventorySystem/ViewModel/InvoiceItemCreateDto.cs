using InventorySystem.Models.BaseEntities;

namespace InventorySystem.ViewModel;

public class InvoiceCreateDto:BaseEntity
{
    public string InvoiceNo { get; set; }
    public long CustomerId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public decimal PaidAmount { get; set; }
    public string Notes { get; set; }

    public List<InvoiceItemCreateDto> Items { get; set; } = new();
}
public class InvoiceItemCreateDto
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal VatPercent { get; set; }
    public string SerialNumber { get; set; } = Guid.NewGuid().ToString("N");
}
