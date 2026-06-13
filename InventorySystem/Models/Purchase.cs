using InventorySystem.Models.BaseEntities;
using static InventorySystem.Models.Auth.IdentityModel;

namespace InventorySystem.Models
{
    public class Purchase : AuditableEntity
    {
        public string InvoiceNo { get; set; }
        public long SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Discount { get; set; }
        public decimal TransportCost { get; set; }
        public string Note { get; set; }
        public ICollection<StockEntry> StockEntries { get; set; }
    }
}
