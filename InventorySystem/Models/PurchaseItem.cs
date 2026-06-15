using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models;

public class PurchaseItem:AuditableEntity
{
    public long PurchaseId { get; set; }
    public Purchase Purchase { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;
}
