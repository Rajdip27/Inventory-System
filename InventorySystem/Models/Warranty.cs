using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models;

public class Warranty: AuditableEntity  
{
    public string WarrantyNo { get; set; }

    public long CustomerId { get; set; }
    public Customer Customer { get; set; }

    public long SalesInvoiceId { get; set; }
    public SalesInvoice SalesInvoice { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; }
    // Active, Expired, ClaimRequested, Replaced, Repaired

    public ICollection<WarrantyItem> WarrantyItem { get; set; }
        = new List<WarrantyItem>();
}
