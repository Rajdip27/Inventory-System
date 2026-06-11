using InventorySystem.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class InvoiceItem:AuditableEntity
    {
        public long InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal PurchasePrice { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        [StringLength(100)]
        public string SerialNumber { get; set; }
        public ICollection<Warranty> Warranty { get; set; } = new List<Warranty>();
        public ICollection<ProductSerial> ProductSerial { get; set; } = new List<ProductSerial>();
    }
}
