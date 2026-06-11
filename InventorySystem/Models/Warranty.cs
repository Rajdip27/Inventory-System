using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models;

public class Warranty: AuditableEntity  
{
    public long InvoiceId { get; set; }
    public Invoice Invoice { get; set; }
    public long InvoiceItemId { get; set; }
    public InvoiceItem InvoiceItem { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public DateTime WarrantyStartDate { get; set; }
    public DateTime WarrantyEndDate { get; set; }
    public string WarrantyStatus { get; set; } = "active";
    public string Notes { get; set; }
}
