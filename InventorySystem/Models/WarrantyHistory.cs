using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models;

public class WarrantyHistory:AuditableEntity
{
    public long WarrantyClaimId { get; set; }
    public WarrantyClaim WarrantyClaim { get; set; }

    public string Action { get; set; }

    public string Remarks { get; set; }

    public DateTime ActionDate { get; set; }

    public long? UserId { get; set; }
}
