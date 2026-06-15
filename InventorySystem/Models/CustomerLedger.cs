using InventorySystem.Models.BaseEntities;

namespace InventorySystem.Models
{
    public class CustomerLedger: AuditableEntity
    {
        public long CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime TransactionDate { get; set; }

        public string ReferenceType { get; set; } // Invoice, Payment, Opening
        public long? ReferenceId { get; set; }

        public string Description { get; set; }

        // Debit = Customer owes you
        public decimal Debit { get; set; }

        // Credit = Customer paid you
        public decimal Credit { get; set; }

        public decimal Balance { get; set; }
    }
}
