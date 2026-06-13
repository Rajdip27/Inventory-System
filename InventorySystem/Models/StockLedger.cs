using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models;

public class StockLedger : AuditableEntity
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    // Purchase, Sale, Return, Adjustment, Damage
    public string ReferenceType { get; set; }
    public long ReferenceId { get; set; }
    public long? StockEntryId { get; set; }
    public StockEntry StockEntry { get; set; }

    public long? SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    // Stock Movement
    public int QuantityIn { get; set; } = 0;
    public int QuantityOut { get; set; } = 0;
    // Running Balance
    public int BalanceQuantity { get; set; }
    // Cost Information
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string Remarks { get; set; }
    public DateTimeOffset EntryDate { get; set; } = DateTimeOffset.Now;
}