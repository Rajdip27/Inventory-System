using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models;

public class WarrantyClaim: AuditableEntity 
{
    public string ClaimNo { get; set; }
    public long WarrantyItemId { get; set; }
    public WarrantyItem WarrantyItem { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; }
    public DateTime ClaimDate { get; set; }
    public string IssueDescription { get; set; }

    public string ActionTaken { get; set; }
    // Replace, Repair, Reject
    public string Status { get; set; }
    // Pending, Approved, Rejected, Completed
    public DateTime? ResolvedDate { get; set; }

    public ICollection<WarrantyHistory> WarrantyHistory { get; set; }
= new List<WarrantyHistory>();
}
