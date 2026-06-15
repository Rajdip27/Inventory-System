using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models;

public class SalesItem:AuditableEntity
{
    public long SalesInvoiceId { get; set; }
    public SalesInvoice SalesInvoice { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;
}
