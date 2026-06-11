using InventorySystem.Models.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models;

public class Product: AuditableEntity
{
    [Required, StringLength(200)]
    public string ProductName { get; set; }
    [StringLength(100)]
    public string Sku { get; set; }
    [StringLength(100)]
    public string Barcode { get; set; }
    public string Description { get; set; }
    [Required]
    public decimal PurchasePrice { get; set; }
    [Required]
    public decimal SellingPrice { get; set; }
    public int WarrantyMonths { get; set; } = 0;
    public bool Status { get; set; } = true;
    public ICollection<StockEntry> StockEntry { get; set; } = new List<StockEntry>();
    public ICollection<StockLedger> StockLedger { get; set; } = new List<StockLedger>();
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<WarrantyClaim> WarrantyClaim { get; set; } = new List<WarrantyClaim>();
    public ICollection<ProductSerial> ProductSerial { get; set; } = new List<ProductSerial>();
}
