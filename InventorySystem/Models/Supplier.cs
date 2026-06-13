using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models
{
    public class Supplier : AuditableEntity
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string TradeLicense { get; set; }
        public string TIN { get; set; }
        public string BIN { get; set; }
        public ICollection<StockEntry> StockEntry { get; set; } = new List<StockEntry>();
    }
}
