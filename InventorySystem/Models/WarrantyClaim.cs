using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models;

public class WarrantyClaim: AuditableEntity 
{
    public long WarrantyId { get; set; }
    public Warranty Warranty { get; set; }

    public long CustomerId { get; set; }
    public Customer Customer { get; set; }

    public long ProductId { get; set; }
    public Product Product { get; set; }

    public long InvoiceId { get; set; }
    public Invoice Invoice { get; set; }

    public DateTime ClaimDate { get; set; } = DateTime.Now;

    public string IssueDescription { get; set; }

    public string ClaimStatus { get; set; } = "pending";

    public string ResolutionNote { get; set; }

    public DateTime? ResolvedDate { get; set; }
}
