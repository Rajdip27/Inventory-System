using InventorySystem.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;
using static InventorySystem.Models.Auth.IdentityModel;

namespace InventorySystem.Models
{
    public class StockEntry : AuditableEntity
    {
        public long SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public long ProductId { get; set; }
        public Product Product { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        // Purchase Information
        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public decimal Discount { get; set; } = 0;

        public decimal TaxAmount { get; set; } = 0;

        public decimal TransportCost { get; set; } = 0;

        // Reference Information
        [StringLength(100)]
        public string InvoiceNo { get; set; }

        [StringLength(100)]
        public string BatchNo { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(255)]
        public string Note { get; set; }

        // Calculated Fields
        public decimal SubTotal => Quantity * PurchasePrice;

        public decimal TotalCost =>
            (Quantity * PurchasePrice)
            - Discount
            + TaxAmount
            + TransportCost;

        // Dates
        public DateTime EntryDate { get; set; } = DateTime.Now;
    }
}