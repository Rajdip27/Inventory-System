using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models;

public class StockLedger: AuditableEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public string ReferenceType { get; set; } // purchase, sale, return, adjustment
    public int ReferenceId { get; set; }

    public int QuantityIn { get; set; } = 0;
    public int QuantityOut { get; set; } = 0;
    public int BalanceQuantity { get; set; }
}
