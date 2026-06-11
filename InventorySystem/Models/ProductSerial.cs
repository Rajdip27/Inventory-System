using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models
{
    public class ProductSerial:AuditableEntity
    {
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public string SerialNumber { get; set; }
        public long? InvoiceItemId { get; set; }
        public InvoiceItem InvoiceItem { get; set; }
        public string Status { get; set; } = "available";
    }
}
