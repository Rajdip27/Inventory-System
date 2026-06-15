using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models
{
    public class WarrantyItem:AuditableEntity
    {
        public long WarrantyId { get; set; }
        public Warranty Warranty { get; set; }

        public long ProductId { get; set; }
        public Product Product { get; set; }

        public string SerialNo { get; set; }

        public int Quantity { get; set; }

        public int WarrantyMonths { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; }
        // Active, Claimed, Replaced, Repaired, Expired

        public ICollection<WarrantyClaim> WarrantyClaim { get; set; }
    = new List<WarrantyClaim>();
    }
}
