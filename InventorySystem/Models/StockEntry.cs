using InventorySystem.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;
using static InventorySystem.Models.Auth.IdentityModel;

namespace InventorySystem.Models
{
    public class StockEntry: AuditableEntity
    {
        public long SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal PurchasePrice { get; set; }
        [StringLength(255)]
        public string Note { get; set; }

        public decimal TotalCost => Quantity * PurchasePrice;
        public DateTime EntryDate { get; set; } = DateTime.Now;
    }
}
